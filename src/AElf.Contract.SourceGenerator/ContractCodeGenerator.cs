using System.Text;
using AElf.Tools;
using ContractGenerator;
using ContractGenerator.Primitives;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AElf.Contract.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ContractCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var textFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".proto"));

        var addedFiles = new List<string>();

        context.RegisterSourceOutput(textFiles, (productionContext, protoFile) =>
        {
            try
            {
                var location = Path.GetDirectoryName(protoFile.Path);
                RunProtoCompile(protoFile.Path);
                var fileDescriptors = GenerateFileDescriptorsFromPbFile(protoFile.Path);
                if (fileDescriptors == null)
                {
                    return;
                }

                var dir = Path.GetDirectoryName(protoFile.Path)?.Split(Path.DirectorySeparatorChar).LastOrDefault();
                var options = ParameterParser.Parse(GetGeneratorOptions(dir));
                var outputFiles = ContractGenerator.ContractGenerator.Generate(fileDescriptors, options);
                var outputFile = outputFiles.FirstOrDefault();
                if (outputFile != null)
                {
                    var fileName = outputFile.Name;
                    var content = outputFile.Content;
                    if (!addedFiles.Contains(fileName))
                    {
                        productionContext.AddSource(fileName, SourceText.From(content, Encoding.UTF8));
                        addedFiles.Add(fileName);
                    }
                }

                var originalFileName = Path.GetFileName(protoFile.Path).LowerUnderscoreToUpperCamel()
                    .Replace(".proto", ".cs");
                if (!addedFiles.Contains(originalFileName))
                {
                    productionContext.AddSource(originalFileName,
                        SourceText.From(File.ReadAllText($"{location}/{originalFileName}"), Encoding.UTF8));
                    addedFiles.Add(originalFileName);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{Path.GetFileName(protoFile.Path)}: {e}");
            }
            finally
            {
                var location = Path.GetDirectoryName(protoFile.Path);
                var depFiles = Directory.EnumerateFiles(CurrentDirectory, "*.protodep");
                foreach (var depFile in depFiles)
                {
                    File.Delete(depFile);
                }

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
        });
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
        return $"{toolsPlatform.Os.ToLowerInvariant()}_{toolsPlatform.Cpu.ToLowerInvariant()}";
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