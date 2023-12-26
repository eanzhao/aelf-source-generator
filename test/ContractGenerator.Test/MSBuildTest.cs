using AElf.Contract.SourceGenerator;
using AElf.Tools;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace ContractGenerator.Test;

public class MSBuildTest
{
    const string protoPath = "/Users/zhaoyiqi/Code/aelf-contract-source-generator/test/AElf.Contract.SourceGenerator.Test/protobuf/contract.proto";

    [Fact]
    public void Test()
    {
        var protobuf = new TaskItem
        {
            ItemSpec = protoPath
        };
        protobuf.SetMetadata("Include", protoPath);
        protobuf.SetMetadata("ContractOutputOptions", "stub");
        
        var protobufList = new ITaskItem[]
        {
            protobuf
        };
            
        // Protobuf_ResolvePlatform
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        var protocDirectory = GetPackagedToolsDirectory(toolsPlatform.Os, toolsPlatform.Cpu);
        var protocFullPath = Path.Combine(protocDirectory, toolsPlatform.Os == "windows" ? "protoc.exe" : "protoc");
        
        // _Protobuf_SelectFiles
        //  _Protobuf_SetProtoRoot
        var findUnderPath = new FindUnderPath
        {
            Path = new TaskItem(Environment.CurrentDirectory),
            Files = Array.Empty<ITaskItem>(),
            BuildEngine = new NaiveBuildEngine()
        };
        findUnderPath.Execute();
        
        // Protobuf_PrepareCompile
        
        // _Protobuf_AugmentLanguageCompile
        
        // _Protobuf_CoreCompile
    }

    private string GetPackagedToolsDirectory(string os, string cpu)
    {
        return Path.Combine(Environment.CurrentDirectory, "tools", $"{os.ToLowerInvariant()}_{cpu.ToLowerInvariant()}");
    }
}