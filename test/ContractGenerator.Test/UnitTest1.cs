using AElf.Contract.SourceGenerator;
using AElf.Tools;
using Google.Protobuf;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ContractGenerator.Test;

public class UnitTest1
{
    const string domainDir = "/Users/zhaoyiqi/Code/aelf-contract-source-generator";
    const string protoPath = "/Users/zhaoyiqi/Code/aelf-contract-source-generator/test/AElf.Contracts.Test/Protobuf/" +
                             "base/acs0.proto";

    [Fact]
    public void ProtoCompileTest()
    {
        var name = Path.GetFileNameWithoutExtension(protoPath);
        var location = Path.GetDirectoryName(protoPath);
        var parentPath = Directory.GetParent(location!)!.ToString();
        var outputDir = $"{location}";
        var compiler = new ProtoCompile
        {
            ToolExe = $"{domainDir}/tools/macosx_x64/protoc",
            Generator = "csharp",
            Protobuf = new ITaskItem[]
            {
                new TaskItem(protoPath)
            },
            ProtoPath = new[]
            {
                location!,
                $"{domainDir}/build/native/include",
                parentPath,
                $"{parentPath}/base",
                $"{parentPath}/message",
            },
            ProtoDepDir = "/Users/zhaoyiqi/Code/aelf-contract-source-generator/test/ContractGenerator.Test/obj/Debug/net6.0/",
            OutputDir = outputDir,
            BuildEngine = new NaiveBuildEngine(),
        };
        compiler.Execute();
        var files = compiler.GeneratedFiles;
        var pbFile = $"{outputDir}/{name}.pb";

        var dir = Path.GetDirectoryName(protoPath)?.Split(Path.DirectorySeparatorChar).LastOrDefault();

        if (files == null) return;
        foreach (var file in files)
        {
            var fileStream = File.OpenRead(pbFile);
            var messageParser = new MessageParser<FileDescriptorSet>(() => new FileDescriptorSet());
            var stream = new CodedInputStream(fileStream);
            var set = messageParser.WithExtensionRegistry(FileDescriptorSetLoader.ExtensionRegistry).ParseFrom(stream);
            var fileDescriptors = FileDescriptorSetLoader.Load(set.File);
            File.Delete(pbFile);
            var options = ParameterParser.Parse(GetGeneratorOptions(dir));
            var outputFiles = ContractGenerator.Generate(fileDescriptors, options);
            foreach (var outputFile in outputFiles)
            {
                var fileName = outputFile.Name;
                var content = outputFile.Content;
            }
        }
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

    [Fact]
    public void ProtoToolsPlatformTest()
    {
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        var pre = $"{toolsPlatform.Os.ToLowerInvariant()}_{toolsPlatform.Cpu.ToLowerInvariant()}";
    }

    [Fact]
    public ITaskItem[] ProtoCompilerOutputsTest()
    {
        var compilerOutputs = new ProtoCompilerOutputs
        {
            Protobuf = new ITaskItem[]
            {
                new TaskItem(protoPath)
            },
            Generator = "csharp"
        };

        compilerOutputs.Execute();
        return compilerOutputs.PossibleOutputs;
    }

    [Fact]
    public void ProtoReadDependenciesTest()
    {
        var readDependencies = new ProtoReadDependencies
        {
            Protobuf = new ITaskItem[]
            {
                new TaskItem(protoPath)
            },
            ProtoDepDir = $"{domainDir}/build/native/include",
            BuildEngine = new NaiveBuildEngine(),
        };
        readDependencies.Execute();
    }
}