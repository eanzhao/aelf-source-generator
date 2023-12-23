using AElf.Tools;

namespace ContractGenerator.Test;

public class MSBuildTest
{
    [Fact]
    public void Test()
    {
        // Protobuf_ResolvePlatform
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        
    }
}