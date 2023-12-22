using AElf.Contract.SourceGenerator;
using AElf.Tools;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ContractGenerator.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        const string domainDir = "/Users/zhaoyiqi/Code/contract-source-generator/src/AElf.Contract.SourceGenerator";
        var protoPath =
            "/Users/zhaoyiqi/Code/contract-source-generator/test/AElf.Contract.SourceGenerator.Test/contract.proto";
        var name = Path.GetFileName(protoPath);
        var location = Path.GetDirectoryName(protoPath);
        var outputDir = $"{location}/Generated";

        var compiler = new ProtoCompile
        {
            ToolExe = $"{domainDir}/tools/macosx_x64/protoc",
            Generator = "CSharp",
            ProtoDepDir = $"{domainDir}/build/native/include/aelf",
            Protobuf = new ITaskItem[] { new TaskItem(protoPath) },
            OutputDir = outputDir,
            ProtoPath = new[]
            {
                location,
                $"{domainDir}/build/native/include"
            },
            BuildEngine = new FakeBuildEngine(),
        };
        compiler.Execute();
        var files = compiler.AdditionalFileWrites;
    }
}