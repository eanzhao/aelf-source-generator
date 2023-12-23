using System.Diagnostics;
using System.Text;
using AElf.Tools;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AElf.Contract.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ContractProtoCodeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Debugger.Launch();

        var currentDirectory = Environment.CurrentDirectory;

        var textFiles = context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".proto"));
        context.RegisterSourceOutput(textFiles, (productionContext, protoFile) =>
        {
            var location = Path.GetDirectoryName(protoFile.Path);

            var compiler = new ProtoCompile
            {
                ToolExe = $"{currentDirectory}/tools/{GetPlatform()}/protoc",
                Generator = "csharp",
                ProtoDepDir = $"{currentDirectory}/build/native/include/aelf",
                Protobuf = new ITaskItem[] { new TaskItem(protoFile.Path) },
                OutputDir = currentDirectory,
                OutputOptions = new[] { "file_extension=.g.cs" },
                ProtoPath = new[]
                {
                    location,
                    $"{currentDirectory}/build/native/include"
                },
                BuildEngine = new NaiveBuildEngine(),
            };
            compiler.Execute();
            if (compiler.GeneratedFiles == null)
            {
                return;
            }

            foreach (var file in compiler.GeneratedFiles)
            {
                var text = Generate(file.ItemSpec);
                productionContext.AddSource(Path.GetFileName(file.ItemSpec), SourceText.From(text, Encoding.UTF8));
                File.Delete(file.ItemSpec);
            }
        });
    }

    private string Generate(string filePath)
    {
        return File.ReadAllText(filePath);
        // using var fileStream = File.Create(filePath);
        // var request = CodeGeneratorRequest.Parser.ParseFrom(fileStream);
        // var fileDescriptors = FileDescriptorSetLoader.Load(request.ProtoFile);
        // return fileDescriptors.First().Name;
    }

    private string GetPlatform()
    {
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        return $"{toolsPlatform.Os.ToLowerInvariant()}_{toolsPlatform.Cpu.ToLowerInvariant()}";
    }
}