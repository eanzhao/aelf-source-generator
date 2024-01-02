using AElf.Contract.SourceGenerator.Generator;
using AElf.Contract.SourceGenerator.Generator.Primitives;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.CodeAnalysis;

namespace AElf.Contract.SourceGenerator;

[Generator]
public class ContractCodeGenerator : IIncrementalGenerator
{
    private static bool _clearedFiles;
    private readonly IAddSourceService _addSourceService = new AddSourceToGeneratedDirectoryService();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var protoFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".proto"));

        var protoFilePaths = protoFiles.Select((proto, _) => proto.Path);

        var addedFiles = new List<string>();

        context.RegisterSourceOutput(protoFilePaths, (productionContext, protoFilePath) =>
        {
            try
            {
                var location = Path.GetDirectoryName(protoFilePath);

                if (!_clearedFiles)
                {
                    var generatedDir =
                        $"{Directory.GetParent(Path.GetDirectoryName(location)!)}{Path.DirectorySeparatorChar}Generated";
                    foreach (var directory in Directory.EnumerateDirectories(generatedDir))
                    {
                        foreach (var generatedFile in Directory.EnumerateFiles(directory))
                        {
                            File.Delete(generatedFile);
                        }
                    }

                    _clearedFiles = true;
                }

                ProtoCompileRunner.Run(protoFilePath);
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
                    }
                }

                var originalFileName = Path.GetFileName(protoFilePath).LowerUnderscoreToUpperCamel()
                    .Replace(".proto", ".cs");
                if (!addedFiles.Contains(originalFileName))
                {
                    _addSourceService.AddSource(productionContext, $"{Path.ChangeExtension(originalFileName, ".g.cs")}",
                        File.ReadAllText($"{location}/{originalFileName}"), protoFilePath);
                    addedFiles.Add(originalFileName);
                }

                if (dir == "contract")
                {
                    var stateTypeName = fileDescriptors.Last().Services.Last().GetOptions()
                        .GetExtension(OptionsExtensions.CsharpState);
                    var stateOutput = new ContractStateGenerator(protoFilePath).Generate(stateTypeName);
                    _addSourceService.AddSource(productionContext, $"{stateOutput.Item1}", stateOutput.Item2, protoFilePath);
                    addedFiles.Add(stateOutput.Item1);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{Path.GetFileName(protoFilePath)}: {e}");
            }
            finally
            {
                var depFiles = Directory.EnumerateFiles(CurrentDirectory, "*.protodep");
                foreach (var depFile in depFiles)
                {
                    File.Delete(depFile);
                }

                var location = Path.GetDirectoryName(protoFilePath);
                if (location != null)
                {
                    var pbFiles = Directory.EnumerateFiles(location, "*.pb");
                    foreach (var pbFile in pbFiles)
                    {
                        File.Delete(pbFile);
                    }

                    var csFiles = Directory.EnumerateFiles(location, "*.cs");
                    foreach (var csFile in csFiles)
                    {
                        File.Delete(csFile);
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
            return null;
        }

        using var fileStream = File.OpenRead(pbFile);
        var messageParser = new MessageParser<FileDescriptorSet>(() => new FileDescriptorSet());
        var set = messageParser.WithExtensionRegistry(FileDescriptorSetLoader.ExtensionRegistry)
            .ParseFrom(new CodedInputStream(fileStream));
        var fileDescriptors = FileDescriptorSetLoader.Load(set.File);
        return fileDescriptors;
    }

    private static string CurrentDirectory => Environment.CurrentDirectory;

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
}