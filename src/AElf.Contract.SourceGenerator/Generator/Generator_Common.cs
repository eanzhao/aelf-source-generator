using Google.Protobuf.Reflection;

namespace ContractGenerator;

public partial class Generator
{
    #region Methods

    private void Methods()
    {
        InRegion("Methods", () =>
        {
            foreach (var method in GetFullMethod()) GenerateStaticMethodField(method);
        });
    }

    /// <summary>
    ///     Generates instantiations for static readonly aelf::Method fields based on the proto
    /// </summary>
    private void GenerateStaticMethodField(MethodDescriptor methodDescriptor)
    {
        var request = ProtoUtils.GetClassName(methodDescriptor.InputType);
        var response = ProtoUtils.GetClassName(methodDescriptor.OutputType);
        PrintLine(
            $"static readonly aelf::Method<{request}, {response}> {GetMethodFieldName(methodDescriptor)} = new " +
            $"aelf::Method<{request}, {response}>(");
        Indent();
        Indent();
        PrintLine($"{GetCSharpMethodType(methodDescriptor)},");
        PrintLine($"{ServiceFieldName},");
        PrintLine($"\"{methodDescriptor.Name}\",");
        PrintLine($"{GetMarshallerFieldName(methodDescriptor.InputType)},");
        PrintLine($"{GetMarshallerFieldName(methodDescriptor.OutputType)});");
        ___EmptyLine___();
        Outdent();
        Outdent();
    }

    #endregion Methods

    #region Descriptors

    private void Descriptors()
    {
        InRegion("Descriptors", () =>
        {
            GenerateServiceDescriptorProperty();
            ___EmptyLine___();
            GenerateAllServiceDescriptorsProperty();
        });
    }

    #endregion Descriptors
}
