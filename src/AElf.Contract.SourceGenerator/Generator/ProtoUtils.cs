using ContractGenerator.Primitives;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public class ProtoUtils
{
    //TODO Implement https://github.com/protocolbuffers/protobuf/blob/e57166b65a6d1d55fc7b18beaae000565f617f22/src/google/protobuf/compiler/csharp/names.cc#L73
    public static string GetClassName(IDescriptor descriptor)
    {
        return ToCSharpName(descriptor.FullName, descriptor.File);
    }

    public static string GetReflectionClassName(FileDescriptor descriptor)
    {
        var result = GetFileNamespace(descriptor);
        if (result.Length > 0) result += '.';
        result += GetReflectionClassUnqualifiedName(descriptor);
        return "global::" + result;
    }

    private static string GetFileNameBase(IDescriptor descriptor)
    {
        var protoFile = descriptor.Name;
        var lastSlash = protoFile.LastIndexOf('/');
        var stringBase = protoFile[(lastSlash + 1)..];
        return StripDotProto(stringBase).UnderscoresToPascalCase();
    }

    private static string StripDotProto(string protoFile)
    {
        var lastIndex = protoFile.LastIndexOf(".", StringComparison.Ordinal);
        return protoFile[..lastIndex];
    }

    public static string GetReflectionClassUnqualifiedName(FileDescriptor descriptor)
    {
        // TODO: Detect collisions with existing messages,
        // and append an underscore if necessary.
        return GetFileNameBase(descriptor) + "Reflection";
    }

    private static string ToCSharpName(string name, FileDescriptor fileDescriptor)
    {
        var result = GetFileNamespace(fileDescriptor);
        if (!string.IsNullOrEmpty(result)) result += '.';

        var classname = string.IsNullOrEmpty(fileDescriptor.Package)
            ? name
            :
            // Strip the proto package from full_name since we've replaced it with
            // the C# namespace.
            name[(fileDescriptor.Package.Length + 1)..];

        classname = classname.Replace(".", ".Types.");
        classname = classname.Replace(".proto", ""); //strip-out the .proto
        return "global::" + result + classname;
    }


    /// <summary>
    ///     Extract the C# Namespace for the target contract based on the Proto data.
    /// </summary>
    //TODO Implementation https://github.com/protocolbuffers/protobuf/blob/e57166b65a6d1d55fc7b18beaae000565f617f22/src/google/protobuf/compiler/csharp/names.cc#L66
    public static string GetFileNamespace(FileDescriptor fileDescriptor)
    {
        // If there has no option csharp_namespace = "XXX" in proto files, state name will return empty string. Such as message proto.
        if (fileDescriptor.GetOptions() == null)
        {
            return "";
        }
        return fileDescriptor.GetOptions().HasCsharpNamespace
            ? fileDescriptor.GetOptions().CsharpNamespace
            : fileDescriptor.Package.UnderscoresToCamelCase(true, true);
    }
}
