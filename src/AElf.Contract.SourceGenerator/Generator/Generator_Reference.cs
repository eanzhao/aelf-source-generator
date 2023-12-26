using ContractGenerator.Primitives;

namespace ContractGenerator;

public partial class Generator
{
    /// <summary>
    ///     Generates the Class for the ReferenceState as part of the aelf contract
    /// </summary>
    protected internal void GenerateReferenceClass()
    {
        PrintLine($"public class {GetReferenceClassName()} : global::AElf.Sdk.CSharp.State.ContractReferenceState");
        InBlock(() =>
        {
            var methods = GetFullMethod();
            foreach (var method in methods)
            {
                var request = ProtoUtils.GetClassName(method.InputType);
                var response = ProtoUtils.GetClassName(method.OutputType);
                PrintLine(
                    $"{Options.GetAccessLevel()} global::AElf.Sdk.CSharp.State.MethodReference<{request}, {response}> {method.Name} {{ get; set; }}");
            }
        });
    }

    private string GetReferenceClassName()
    {
        return _serviceDescriptor.Name + "ReferenceState";
    }
}
