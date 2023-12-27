using AElf;
using Google.Protobuf.Reflection;

namespace AElf.Contract.SourceGenerator.Generator;

public partial class Generator : AbstractGenerator
{
    private const string ServiceFieldName = "__ServiceName";
    private ServiceDescriptor _serviceDescriptor;

    public Generator(ServiceDescriptor serviceDescriptor, GeneratorOptions options) : base(options)
    {
        _serviceDescriptor = serviceDescriptor;
    }

    /// <summary>
    ///     Generate will produce a chunk of C# code that serves as the container class of the AElf Contract.
    /// </summary>
    public override string? Generate()
    {
        // GenerateDocCommentBody(serviceDescriptor,)
        PrintLine($"{AccessLevel} static partial class {ServiceContainerClassName}");
        InBlock(() =>
        {
            PrintLine(@$"static readonly string {ServiceFieldName} = ""{_serviceDescriptor.FullName}"";");

            ___EmptyLine___();
            Marshallers();
            ___EmptyLine___();
            Methods();
            ___EmptyLine___();
            Descriptors();

            if (Options.GenerateContract)
            {
                ___EmptyLine___();
                GenerateContractBaseClass();
                ___EmptyLine___();
                GenerateBindServiceMethod();
            }

            if (Options.GenerateStub)
            {
                ___EmptyLine___();
                GenerateStubClass();
            }

            if (Options.GenerateReference)
            {
                ___EmptyLine___();
                GenerateReferenceClass();
            }
        });
        return PrintOut();
    }

    private void GenerateAllServiceDescriptorsProperty()
    {
        PrintLine(
            "public static global::System.Collections.Generic.IReadOnlyList<global::Google.Protobuf.Reflection.ServiceDescriptor> Descriptors"
        );
        InBlock(() =>
        {
            PrintLine("get");
            InBlock(() =>
            {
                PrintLine(
                    "return new global::System.Collections.Generic.List<global::Google.Protobuf.Reflection.ServiceDescriptor>()");
                InBlockWithSemicolon(() =>
                {
                    var services = _serviceDescriptor.GetFullService();
                    foreach (var service in services)
                    {
                        var index = service.Index.ToString();
                        PrintLine(
                            $"{ProtoUtils.GetReflectionClassName(service.File)}.Descriptor.Services[{index}],");
                    }
                });
            });
        });
    }

    private string GetServerClassName()
    {
        return _serviceDescriptor.Name + "Base";
    }

    /// <summary>
    ///     Generates a section of instantiated aelf Marshallers as part of the contract
    /// </summary>
    private void Marshallers()
    {
        PrintLine("#region Marshallers");
        var usedMessages = GetUsedMessages();
        foreach (var usedMessage in usedMessages)
        {
            var fieldName = GetMarshallerFieldName(usedMessage);
            var t = ProtoUtils.GetClassName(usedMessage);
            PrintLine(
                $"static readonly aelf::Marshaller<{t}> {fieldName} = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), {t}.Parser.ParseFrom);");
        }

        PrintLine("#endregion");
    }


    private void GenerateServiceDescriptorProperty()
    {
        PrintLine(
            "public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor");
        PrintLine("{");
        PrintLine(
            $"  get {{ return {ProtoUtils.GetReflectionClassName(_serviceDescriptor.File)}.Descriptor.Services[{_serviceDescriptor.Index}]; }}");
        PrintLine("}");
    }


    /// <summary>
    ///     GetMarshallerFieldName formats and returns a marshaller-fieldname based on the original C++ logic
    ///     found here
    ///     https://github.com/AElfProject/contract-plugin/blob/de625fcb79f83603e29d201c8488f101b40f573c/src/contract_csharp_generator.cc#L242
    /// </summary>
    private static string GetMarshallerFieldName(IDescriptor message)
    {
        var msgFullName = message.FullName;
        return "__Marshaller_" + msgFullName.Replace(".", "_");
    }

    private List<MethodDescriptor> GetFullMethod()
    {
        return _serviceDescriptor.GetFullService().SelectMany(serviceItem => serviceItem.Methods).ToList();
    }

    private static string GetMethodFieldName(MethodDescriptor method)
    {
        return "__Method_" + method.Name;
    }

    private static string GetCSharpMethodType(MethodDescriptor method)
    {
        return IsViewOnlyMethod(method) ? "aelf::MethodType.View" : "aelf::MethodType.Action";
    }


    private static bool IsViewOnlyMethod(MethodDescriptor method)
    {
        if (method.GetOptions() == null)
        {
            return false;
        }
        return method.GetOptions().GetExtension(OptionsExtensions.IsView);
    }

    /// <summary>
    ///     GetUsedMessages extracts messages from Proto ServiceDescriptor based on the original C++ logic
    ///     found here
    ///     https://github.com/AElfProject/contract-plugin/blob/de625fcb79f83603e29d201c8488f101b40f573c/src/contract_csharp_generator.cc#L312
    /// </summary>
    private List<IDescriptor> GetUsedMessages()
    {
        var descriptorSet = new HashSet<IDescriptor>();
        var result = new List<IDescriptor>();

        var methods = GetFullMethod();
        foreach (var method in methods)
        {
            if (!descriptorSet.Contains(method.InputType))
            {
                descriptorSet.Add(method.InputType);
                result.Add(method.InputType);
            }

            if (descriptorSet.Contains(method.OutputType)) continue;
            descriptorSet.Add(method.OutputType);
            result.Add(method.OutputType);
        }

        return result;
    }


    private enum MethodType
    {
        MethodtypeNoStreaming,
        MethodtypeClientStreaming,
        MethodtypeServerStreaming,
        MethodtypeBidiStreaming
    }

    private string AccessLevel => Options.InternalAccess ? "internal" : "public";

    private string ServiceContainerClassName => $"{_serviceDescriptor.Name}Container";
}
