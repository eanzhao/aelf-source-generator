using AElf;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace AElf.Contract.SourceGenerator;

public static class FileDescriptorSetLoader
{
    private static readonly ExtensionRegistry ExtensionRegistry = new();

    static FileDescriptorSetLoader()
    {
        ExtensionRegistry.Add(OptionsExtensions.Identity);
        ExtensionRegistry.Add(OptionsExtensions.Base);
        ExtensionRegistry.Add(OptionsExtensions.CsharpState);
        ExtensionRegistry.Add(OptionsExtensions.IsView);
        ExtensionRegistry.Add(OptionsExtensions.IsEvent);
        ExtensionRegistry.Add(OptionsExtensions.IsIndexed);
    }

    public static IReadOnlyList<FileDescriptor> Load(Stream stream)
    {
        var fds = FileDescriptorSet.Parser.WithExtensionRegistry(ExtensionRegistry).ParseFrom(stream);
        return Load(fds.File);
    }

    public static IReadOnlyList<FileDescriptor> Load(IEnumerable<FileDescriptorProto> protos)
    {
        var fileInByteStrings = protos.Select(f => f.ToByteString());
        return FileDescriptor.BuildFromByteStrings(fileInByteStrings, ExtensionRegistry);
    }
}
