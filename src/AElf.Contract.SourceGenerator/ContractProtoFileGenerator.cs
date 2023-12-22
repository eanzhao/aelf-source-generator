using System.Text;
using AElf.Contract.SourceGenerator.Extensions;
using AElf.Tools;
using Google.Protobuf.Compiler;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AElf.Contract.SourceGenerator;

[Generator]
public class ContractProtoFileGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var protoFiles = context.AdditionalFiles
            .Where(at => at.Path.EndsWith(".proto"))
            .ToList();

        var os = CommonPlatformDetection.GetOSKind().ToString().ToLowerInvariant();
        var cpu = CommonPlatformDetection.GetProcessArchitecture().ToString().ToLowerInvariant();

        var generator = new ContractGenerator();

        var domainDir = Environment.CurrentDirectory;

        foreach (var protoFile in protoFiles)
        {
            var name = Path.GetFileName(protoFile.Path);
            var location = Path.GetDirectoryName(protoFile.Path);
            var userOptions = context.AnalyzerConfigOptions.GetOptions(protoFile);
            var generateType = userOptions.GetGenerateType();

            var outputDir = Environment.CurrentDirectory; //$"{location}/Generated";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var compiler = new ProtoCompile
            {
                ToolExe = $"{domainDir}/tools/macosx_x64/protoc",
                Generator = "csharp",
                ProtoDepDir = $"{domainDir}/build/native/include/aelf",
                Protobuf = new ITaskItem[] { new TaskItem(protoFile.Path) },
                OutputDir = outputDir,
                OutputOptions = new[] { "file_extension=.g.cs" },
                ProtoPath = new[]
                {
                    location,
                    $"{domainDir}/build/native/include"
                },
                BuildEngine = new FakeBuildEngine(),
                ContractPluginExe =
                    $"{domainDir}/tools/macosx_x64/contract_csharp_plugin",
                ContractOutputOptions = new[] { "reference" }
            };
            compiler.Execute();

            if (compiler.GeneratedFiles == null) continue;
            foreach (var file in compiler.GeneratedFiles)
            {
                var text = Generate(file.ItemSpec);
                context.AddSource(Path.GetFileName(file.ItemSpec), SourceText.From(text, Encoding.UTF8));
                File.Delete(file.ItemSpec);
            }
        }
    }

    private string Generate(string filePath)
    {
        return File.ReadAllText(filePath);
        // using var fileStream = File.Create(filePath);
        // var request = CodeGeneratorRequest.Parser.ParseFrom(fileStream);
        // var fileDescriptors = FileDescriptorSetLoader.Load(request.ProtoFile);
        // return fileDescriptors.First().Name;
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}