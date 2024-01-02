using AElf.Tools;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace AElf.Contract.SourceGenerator;

public class ProtoCompileRunner
{
    private static string CurrentDirectory => Environment.CurrentDirectory;

    public static void Run(string protoFilePath)
    {
        var location = Path.GetDirectoryName(protoFilePath);
        var parentPath = Directory.GetParent(location!)!.ToString();
        var compiler = new ProtoCompile
        {
            ToolExe = GetToolExePath(),
            Generator = "csharp",
            Protobuf = new ITaskItem[]
            {
                new TaskItem(protoFilePath)
            },
            ProtoPath = new[]
            {
                location!,
                $"{CurrentDirectory}/build/native/include",
                parentPath,
                $"{parentPath}/base",
                $"{parentPath}/message",
            },
            ProtoDepDir = CurrentDirectory,
            OutputDir = location,
            BuildEngine = new NaiveBuildEngine(),
        };
        compiler.Execute();
    }

    private static string GetToolExePath()
    {
        return $"{CurrentDirectory}/tools/{GetPlatform()}/protoc";
    }

    private static string GetPlatform()
    {
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        return $"{toolsPlatform.Os!.ToLowerInvariant()}_{toolsPlatform.Cpu!.ToLowerInvariant()}";
    }
}