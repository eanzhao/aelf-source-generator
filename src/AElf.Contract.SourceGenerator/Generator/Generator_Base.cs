using AElf;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public partial class Generator
{
    /// <summary>
    ///     Generate will produce a chunk of C# code BaseClass for the AElf Contract. based on C++ original
    ///     https://github.com/AElfProject/contract-plugin/blob/453bebfec0dd2fdcc06d86037055c80721d24e8a/src/contract_csharp_generator.cc#L422
    /// </summary>
    protected internal void GenerateContractBaseClass()
    {
        var serverClassName = GetServerClassName();
        PrintLine(
            $"/// <summary>Base class for the contract of {serverClassName}</summary>");
        PrintLine(
            $"public abstract partial class {serverClassName} : AElf.Sdk.CSharp.CSharpSmartContract<{GetStateTypeName()}>");
        InBlock(() =>
        {
            var methods = GetFullMethod();
            foreach (var method in methods)
            {
#if VIRTUAL_METHOD
                PrintLine(
                    $"public virtual {GetMethodReturnTypeServer(method)} {method.Name}({GetMethodRequestParamServer(method)}{GetMethodResponseStreamMaybe(method)})");
                PrintLine("{");
                Indent();
                PrintLine("throw new global::System.NotImplementedException();");
                Outdent();
                PrintLine("}");
#else
                PrintLine(
                      $"public abstract {GetMethodReturnTypeServer(method)} {method.Name}({GetMethodRequestParamServer(method)}{GetMethodResponseStreamMaybe(method)});");
#endif
            }
        });
    }

    private void GenerateBindServiceMethod()
    {
        PrintLine(
            $"public static aelf::ServerServiceDefinition BindService({GetServerClassName()} serviceImpl)");
        InBlock(() =>
        {
            PrintLine("return aelf::ServerServiceDefinition.CreateBuilder()");
            Indent();
            Indent();
            PrintLine(".AddDescriptors(Descriptors)");
            var methods = GetFullMethod();
            if (methods.Count > 0)
            {
                foreach (var method in methods.SkipLast(1))
                {
                    PrintLine(
                        $".AddMethod({GetMethodFieldName(method)}, serviceImpl.{method.Name})");
                }

                var lastMethod = methods.Last();
                PrintLine(
                    $".AddMethod({GetMethodFieldName(lastMethod)}, serviceImpl.{lastMethod.Name}).Build();");
            }
            Outdent();
            Outdent();
        });
        ___EmptyLine___();
    }

    #region Helper Methods

    private string GetStateTypeName()
    {
        // If there has no option (aelf.csharp_state) = "XXX" in proto files, state name will return empty string. Such as base proto.
        if (_serviceDescriptor.GetOptions() == null)
        {
            return "";
        }
        return _serviceDescriptor.GetOptions().GetExtension(OptionsExtensions.CsharpState);
    }


    private static string GetMethodReturnTypeServer(MethodDescriptor method)
    {
        return ProtoUtils.GetClassName(method.OutputType);
    }

    private static string GetMethodRequestParamServer(MethodDescriptor method)
    {
        switch (GetMethodType(method))
        {
            case MethodType.MethodtypeNoStreaming:
            case MethodType.MethodtypeServerStreaming:
                return ProtoUtils.GetClassName(method.InputType) + " input";
            case MethodType.MethodtypeClientStreaming:
            case MethodType.MethodtypeBidiStreaming:
                return "grpc::IAsyncStreamReader<" + ProtoUtils.GetClassName(method.InputType) +
                       "> requestStream";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static string GetMethodResponseStreamMaybe(MethodDescriptor method)
    {
        switch (GetMethodType(method))
        {
            case MethodType.MethodtypeNoStreaming:
            case MethodType.MethodtypeClientStreaming:
                return "";
            case MethodType.MethodtypeServerStreaming:
            case MethodType.MethodtypeBidiStreaming:
                return ", grpc::IServerStreamWriter<" +
                       ProtoUtils.GetClassName(method.OutputType) + "> responseStream";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private static MethodType GetMethodType(MethodDescriptor method)
    {
        if (method.IsClientStreaming)
            return method.IsServerStreaming ? MethodType.MethodtypeBidiStreaming : MethodType.MethodtypeClientStreaming;
        return method.IsServerStreaming ? MethodType.MethodtypeServerStreaming : MethodType.MethodtypeNoStreaming;
    }

    #endregion
}
