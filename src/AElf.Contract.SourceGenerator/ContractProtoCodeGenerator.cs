using System.Diagnostics;
using System.Text;
using AElf.Tools;
using Google.Protobuf;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AElf.Contract.SourceGenerator;

//[Generator(LanguageNames.CSharp)]
public class ContractProtoCodeGenerator : IIncrementalGenerator
{
    const string domainDir = "/Users/zhaoyiqi/Code/aelf-contract-source-generator";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Debugger.Launch();

        var textFiles = context.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".proto"));
        context.RegisterSourceOutput(textFiles, (productionContext, protoFile) =>
        {
            var location = Path.GetDirectoryName(protoFile.Path);

            var compiler = new ProtoCompile
            {
                ToolExe = $"{domainDir}/tools/macosx_x64/protoc",
                //ToolExe = $"{CurrentDirectory}/tools/{GetPlatform()}/protoc",
                Generator = "csharp",
                Protobuf = new ITaskItem[] { new TaskItem(protoFile.Path) },
                ProtoPath = new[]
                {
                    location,
                    //$"{CurrentDirectory}/build/native/include"
                    $"{domainDir}/build/native/include",
                },
                ProtoDepDir = $"{domainDir}/build",
                OutputDir = Path.GetDirectoryName(protoFile.Path),
                //OutputOptions = new[] { "file_extension=.g.cs" },
                BuildEngine = new NaiveBuildEngine(),
            };
            compiler.Execute();
            var generatedFiles = compiler.GeneratedFiles;
            if (generatedFiles == null)
            {
                File.Create($"/Users/zhaoyiqi/Code/aelf-contract-source-generator/{Path.GetFileNameWithoutExtension(protoFile.Path)}-no.txt");
                return;
            }

            foreach (var file in generatedFiles)
            {
                File.Create($"/Users/zhaoyiqi/Code/aelf-contract-source-generator/aaaaaa{file.ItemSpec}.txt");
                var protoFileName = Path.GetFileNameWithoutExtension(file.ItemSpec);
                var responseFiles = Generate(protoFileName, new[] { "stub" });
                foreach (var responseFile in responseFiles)
                {
                    File.Create($"/Users/zhaoyiqi/Code/aelf-contract-source-generator/--{responseFile.Name}.txt");
            
                    productionContext.AddSource(responseFile.Name,
                        SourceText.From(responseFile.Content, Encoding.UTF8));
                }
                File.Delete(file.ItemSpec);
            }
        });
    }

    private IReadOnlyList<CodeGeneratorResponse.Types.File> Generate(string protoFileName, string[] parameters)
    {
        var pbFile = $"{CurrentDirectory}/{protoFileName}.pb";
        var fileStream = File.OpenRead(pbFile);
        var messageParser = new MessageParser<FileDescriptorSet>(() => new FileDescriptorSet());
        var stream = new CodedInputStream(fileStream);
        var set = messageParser.WithExtensionRegistry(FileDescriptorSetLoader.ExtensionRegistry).ParseFrom(stream);
        var fileDescriptors = FileDescriptorSetLoader.Load(set.File);
        File.Delete(pbFile);
        var options = ParameterParser.Parse(parameters);
        return ContractGenerator.Generate(fileDescriptors, options);
    }

    public string CurrentDirectory => "/Users/zhaoyiqi/Code/aelf-contract-source-generator";//Environment.CurrentDirectory;

    private string GetPlatform()
    {
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        return $"{toolsPlatform.Os.ToLowerInvariant()}_{toolsPlatform.Cpu.ToLowerInvariant()}";
    }
}