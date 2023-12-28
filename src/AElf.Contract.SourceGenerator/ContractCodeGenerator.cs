using AElf.Contract.SourceGenerator.Generator;
using AElf.Contract.SourceGenerator.Generator.Primitives;
using AElf.Tools;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;

namespace AElf.Contract.SourceGenerator;

[Generator]
public class ContractCodeGenerator : IIncrementalGenerator
{
    private static bool _clearedFiles;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var protoFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".proto"));

        var addedFiles = new List<string>();

        context.RegisterSourceOutput(protoFiles, (productionContext, protoFile) =>
        {
            try
            {
                var location = Path.GetDirectoryName(protoFile.Path);

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

                RunProtoCompile(protoFile.Path);
                var fileDescriptors = GenerateFileDescriptorsFromPbFile(protoFile.Path);
                if (fileDescriptors == null)
                {
                    return;
                }

                var dir = Path.GetDirectoryName(protoFile.Path)?.Split(Path.DirectorySeparatorChar).LastOrDefault();
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
                        AddSource(productionContext, $"{fileName}", content, protoFile.Path);
                        addedFiles.Add(fileName);
                    }
                }

                var originalFileName = Path.GetFileName(protoFile.Path).LowerUnderscoreToUpperCamel()
                    .Replace(".proto", ".cs");
                if (!addedFiles.Contains(originalFileName))
                {
                    AddSource(productionContext, $"{originalFileName.Replace(".cs", ".g.cs")}",
                        File.ReadAllText($"{location}/{originalFileName}"), protoFile.Path);
                    addedFiles.Add(originalFileName);
                }

                if (dir == "contract")
                {
                    var stateTypeName = fileDescriptors.Last().Services.Last().GetOptions()
                        .GetExtension(OptionsExtensions.CsharpState);
                    var stateOutput = new ContractStateGenerator().Generate(stateTypeName);
                    AddSource(productionContext, $"{stateOutput.Item1}", stateOutput.Item2, protoFile.Path);
                    addedFiles.Add(stateOutput.Item1);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{Path.GetFileName(protoFile.Path)}: {e}");
            }
            finally
            {
                var depFiles = Directory.EnumerateFiles(CurrentDirectory, "*.protodep");
                foreach (var depFile in depFiles)
                {
                    File.Delete(depFile);
                }

                var location = Path.GetDirectoryName(protoFile.Path);
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

    private void AddSource(SourceProductionContext spc, string name, string text, string protoFilePath)
    {
        var path = $"{Path.GetDirectoryName(protoFilePath)}{Path.DirectorySeparatorChar}{name}";
        if (!path.EndsWith(".cs"))
        {
            path += ".cs";
        }

        path = path.Replace($"{Path.DirectorySeparatorChar}Proto{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}Generated{Path.DirectorySeparatorChar}");
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        }

        File.WriteAllText(path, text);
        //spc.AddSource(name, SourceText.From(text, Encoding.UTF8));
    }

    private void RunProtoCompile(string protoFilePath)
    {
        var location = Path.GetDirectoryName(protoFilePath);
        var parentPath = Directory.GetParent(location!)!.ToString();
        var compiler = new ProtoCompile
        {
            ToolExe = GetToolExePath(),
            Generator = "csharp",
            Protobuf = new ITaskItem[]
            {
                new TaskItem(protoFilePath)
            },
            ProtoPath = new[]
            {
                location!,
                $"{CurrentDirectory}/build/native/include",
                parentPath,
                $"{parentPath}/base",
                $"{parentPath}/message",
            },
            ProtoDepDir = CurrentDirectory,
            OutputDir = location,
            BuildEngine = new NaiveBuildEngine(),
        };
        compiler.Execute();
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

    private string GetToolExePath()
    {
        return $"{CurrentDirectory}/tools/{GetPlatform()}/protoc";
    }

    private static string GetPlatform()
    {
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        return $"{toolsPlatform.Os!.ToLowerInvariant()}_{toolsPlatform.Cpu!.ToLowerInvariant()}";
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
}