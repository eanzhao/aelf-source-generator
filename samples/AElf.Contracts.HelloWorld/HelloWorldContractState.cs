using AElf.Sdk.CSharp.State;

namespace AElf.Contracts.HelloWorld;

public partial class HelloWorldState
{
    public StringState Message { get; set; }
    
    /// <summary>
    /// Order -> Message
    /// </summary>
    public MappedState<int, string> Messages { get; set; }
}