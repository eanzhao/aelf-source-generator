namespace ContractGenerator;

public partial class Generator
{
    private void GenerateStubClass()
    {
        PrintLine($"public class {GetStubClassName()} : aelf::ContractStubBase");
        PrintLine("{");
        {
            Indent();
            var methods = GetFullMethod();
            foreach (var method in methods)
            {
                PrintLine(
                    $"public aelf::IMethodStub<{ProtoUtils.GetClassName(method.InputType)}, {ProtoUtils.GetClassName(method.OutputType)}> {method.Name}");
                PrintLine("{");
                {
                    Indent();
                    PrintLine($"get {{ return __factory.Create({GetMethodFieldName(method)}); }}");
                    Outdent();
                }
                PrintLine("}");
                ___EmptyLine___();
            }

            Outdent();
        }
        PrintLine("}");
    }

    private string GetStubClassName()
    {
        return _serviceDescriptor.Name + "Stub";
    }
}
