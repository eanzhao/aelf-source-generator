using AElf.Contract.SourceGenerator;
using AElf.Tools;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ContractGenerator.Test;

public class UnitTest1
{
    const string domainDir = "/Users/zhaoyiqi/Code/aelf-contract-source-generator";
    const string protoPath = "/Users/zhaoyiqi/Code/aelf-contract-source-generator/test/AElf.Contract.SourceGenerator.Test/protobuf/contract.proto";

    [Fact]
    public void ProtoCompileTest()
    {
        ProtoCompilerOutputsTest();
        ProtoReadDependenciesTest();
        var location = Path.GetDirectoryName(protoPath);
        var outputDir = $"{location}";
        var compiler = new ProtoCompile
        {
            ToolExe = $"{domainDir}/tools/macosx_x64/protoc",
            Generator = "csharp",
            ProtoDepDir = $"{domainDir}/build/native/include",
            Protobuf = new ITaskItem[]
            {
                new TaskItem(protoPath)
            },
            OutputDir = outputDir,
            ProtoPath = new[]
            {
                location,
                $"{domainDir}/build/native/include",
            },
            BuildEngine = new NaiveBuildEngine(),
        };
        compiler.Execute();
        var files = compiler.GeneratedFiles;
        var pbFile = $"{outputDir}/set.pb";

        if (files == null) return;
        foreach (var file in files)
        {
            var fileStream = File.OpenRead(pbFile);
            var messageParser = new MessageParser<FileDescriptorSet>(() => new FileDescriptorSet());
            var stream = new CodedInputStream(fileStream);
            var set = messageParser.WithExtensionRegistry(FileDescriptorSetLoader.ExtensionRegistry).ParseFrom(stream);
            var fileDescriptors = FileDescriptorSetLoader.Load(set.File);
        }
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