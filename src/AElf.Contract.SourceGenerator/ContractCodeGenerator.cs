using System.Collections.Immutable;
using System.Reflection;
using AElf.Contract.SourceGenerator.Generator;
using AElf.Contract.SourceGenerator.Generator.Primitives;
using AElf.Contract.SourceGenerator.Logging;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AElf.Contract.SourceGenerator;

[Generator]
public class ContractCodeGenerator : IIncrementalGenerator
{
    private static bool _clearedFiles;
    private readonly IAddSourceService _addSourceService = new AddSourceToGeneratedDirectoryService();
    private ILogger _logger = NullLogger.Instance;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
            SetupLogger(context);
        }
        catch
        {
            // ignored
        }
        var protoFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".proto"));

        var protoFilePaths = protoFiles.Select((proto, _) => proto.Path);

        var addedFiles = new List<string>();

        context.RegisterSourceOutput(protoFilePaths, (productionContext, protoFilePath) =>
        {
            try
            {
                var location = Path.GetDirectoryName(protoFilePath);
                var name = Path.GetFileName(protoFilePath);

                _logger.Log(LogLevel.Information, $"Dealing with: {name}");

                if (!_clearedFiles)
                {
                    // TODO: Move this logic to IAddSourceService
                    var generatedDir =
                        $"{Directory.GetParent(Path.GetDirectoryName(location)!)}{Path.DirectorySeparatorChar}Generated";
                    if (Directory.Exists(generatedDir))
                    {
                        foreach (var directory in Directory.EnumerateDirectories(generatedDir))
                        {
                            foreach (var generatedFile in Directory.EnumerateFiles(directory))
                            {
                                _logger.Log(LogLevel.Debug, $"Deleted old file: {generatedFile}");
                                File.Delete(generatedFile);
                            }
                        }

                        _clearedFiles = true;
                    }
                }

                ProtoCompileRunner.Run(protoFilePath, _logger);

                var fileDescriptors = GenerateFileDescriptorsFromPbFile(protoFilePath);

                if (fileDescriptors == null)
                {
                    return;
                }

                var dir = Path.GetDirectoryName(protoFilePath)?.Split(Path.DirectorySeparatorChar).LastOrDefault();
                var options = ParameterParser.Parse(GetGeneratorOptions(dir));
                var contractGenerator = new ContractGenerator();
                var outputFiles = contractGenerator.Generate(fileDescriptors, options).ToList();
                var outputFile = outputFiles.FirstOrDefault();
                if (outputFile != null)
                {
                    var fileName = outputFile.Name;
                    var content = outputFile.Content;
                    if (!addedFiles.Contains(fileName))
                    {
                        _addSourceService.AddSource(productionContext, $"{fileName}", content, protoFilePath);
                        addedFiles.Add(fileName);
                        _logger.Log(LogLevel.Information, $"Generated: {fileName}");
                    }
                }

                var originalFileName = name.LowerUnderscoreToUpperCamel().Replace(".proto", ".cs");
                if (!addedFiles.Contains(originalFileName))
                {
                    var generatedFileName = $"{Path.ChangeExtension(originalFileName, ".g.cs")}";
                    _addSourceService.AddSource(productionContext, generatedFileName,
                        File.ReadAllText($"{location}/{originalFileName}"), protoFilePath);
                    addedFiles.Add(originalFileName);
                    _logger.Log(LogLevel.Information, $"Generated: {generatedFileName}");
                }

                if (dir == "contract")
                {
                    _logger.Log(LogLevel.Information, $"Treat {name} as a contract.");
                    var stateTypeName = fileDescriptors.Last().Services.Last().GetOptions()
                        .GetExtension(OptionsExtensions.CsharpState);
                    var stateOutput = new ContractStateGenerator(protoFilePath).Generate(stateTypeName);
                    _addSourceService.AddSource(productionContext, $"{stateOutput.Item1}", stateOutput.Item2,
                        protoFilePath);
                    addedFiles.Add(stateOutput.Item1);
                    _logger.Log(LogLevel.Information, $"Generated: {stateOutput.Item1}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{Path.GetFileName(protoFilePath)}: {e}");
            }
            finally
            {
                var location = Path.GetDirectoryName(protoFilePath);

                var currentDirectory = Directory.GetParent(location!)!.Parent!.Parent!.ToString();
                var depFiles = Directory.EnumerateFiles(currentDirectory, "*.protodep");
                foreach (var depFile in depFiles)
                {
                    File.Delete(depFile);
                    _logger.Log(LogLevel.Debug, $"Deleted {Path.GetFileName(depFile)}");
                }

                if (location != null)
                {
                    var pbFiles = Directory.EnumerateFiles(location, "*.pb");
                    foreach (var pbFile in pbFiles)
                    {
                        File.Delete(pbFile);
                        _logger.Log(LogLevel.Debug, $"Deleted {Path.GetFileName(pbFile)}");
                    }

                    var csFiles = Directory.EnumerateFiles(location, "*.cs");
                    foreach (var csFile in csFiles)
                    {
                        File.Delete(csFile);
                        _logger.Log(LogLevel.Debug, $"Deleted {Path.GetFileName(csFile)}");
                    }
                }
            }
        });
    }

    private IReadOnlyList<FileDescriptor>? GenerateFileDescriptorsFromPbFile(string protoFilePath)
    {
        var location = Path.GetDirectoryName(protoFilePath);
        var protoFileName = Path.GetFileNameWithoutExtension(protoFilePath);
        var pbFile = $"{location}/{protoFileName}.pb";
        if (!File.Exists(pbFile))
        {
            _logger.Log(LogLevel.Error, $"Failed to find {pbFile}");
            return null;
        }

        using var fileStream = File.OpenRead(pbFile);
        var messageParser = new MessageParser<FileDescriptorSet>(() => new FileDescriptorSet());
        var set = messageParser.WithExtensionRegistry(FileDescriptorSetLoader.ExtensionRegistry)
            .ParseFrom(new CodedInputStream(fileStream));
        var fileDescriptors = FileDescriptorSetLoader.Load(set.File);
        return fileDescriptors;
    }

    private string[] GetGeneratorOptions(string? dir)
    {
        if (dir == null)
        {
            return Array.Empty<string>();
        }

        switch (dir)
        {
            case "base":
            case "message":
                return new[] { "nocontract" };
            case "reference":
                return new[] { "reference", "internal_access" };
            case "stub":
                return new[] { "stub", "internal_access" };
        }

        return Array.Empty<string>();
    }

    private void SetupLogger(IncrementalGeneratorInitializationContext context)
    {
        var optionsProvider = context.AnalyzerConfigOptionsProvider.Select((options, _) =>
        {
            var loggingOptions = GetLoggingOptions(options);
            return loggingOptions;
        });
        var logging = optionsProvider
            .Select((options, _) => options)
            .Select((options, _) =>
            {
                _logger = options is null
                    ? NullLogger.Instance
                    : new Logger(options.Value.Level, options.Value.FilePath);
                return 0;
            })
            .SelectMany((_, _) => ImmutableArray<int>.Empty); // don't emit anything

        context.RegisterSourceOutput(logging, static (_, _) =>
        {
            // This delegate will never be called
        });
    }

    private static LoggingOptions? GetLoggingOptions(AnalyzerConfigOptionsProvider options)
    {
        if (!options.GlobalOptions.TryGetValue(SourceGeneratorConstants.LogFilePathProperty,
                out var logFilePath))
            return null;

        if (string.IsNullOrWhiteSpace(logFilePath))
            return null;

        logFilePath = logFilePath.Trim();

        if (!options.GlobalOptions.TryGetValue(SourceGeneratorConstants.LogLevelProperty, out var logLevelValue)
            || !Enum.TryParse(logLevelValue, true, out LogLevel logLevel))
        {
            logLevel = LogLevel.Information;
        }

        return new LoggingOptions(logFilePath, logLevel);
    }
}