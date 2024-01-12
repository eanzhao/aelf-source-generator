using System.Reflection;
using AElf.Tools;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using AElf.Contract.SourceGenerator.Logging;
using ILogger = AElf.Contract.SourceGenerator.Logging.ILogger;

namespace AElf.Contract.SourceGenerator;

public class ProtoCompileRunner
{
    private static string? _currentDirectory;

    public static void Run(string protoFilePath, ILogger? logger = null)
    {
        var location = Path.GetDirectoryName(protoFilePath);
        var parentPath = Directory.GetParent(location!)!.ToString();
        _currentDirectory = Directory.GetParent(location!)!.Parent!.Parent!.ToString();
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
                $"{_currentDirectory}/build/native/include",
                parentPath,
                $"{parentPath}/base",
                $"{parentPath}/message",
            },
            ProtoDepDir = _currentDirectory,
            OutputDir = location,
            BuildEngine = new NaiveBuildEngine(),
        };
        if (compiler.Execute())
        {
            logger?.Log(LogLevel.Debug, "protoc executed.");
            logger?.Log(LogLevel.Error, $"Current Directory: {_currentDirectory}");
        }
        else
        {
            logger?.Log(LogLevel.Error, "Failed to execute protoc");
            logger?.Log(LogLevel.Error, $"Current Directory: {_currentDirectory}");
            logger?.Log(LogLevel.Error, $"Parent Path: {parentPath}");
        }
    }

    private static string GetToolExePath()
    {
        var platform = GetPlatform();
        return $"{_currentDirectory}/tools/{platform}/protoc" + (platform.StartsWith("win") ? ".exe" : "");
    }

    private static string GetPlatform()
    {
        var toolsPlatform = new ProtoToolsPlatform();
        toolsPlatform.Execute();
        return $"{toolsPlatform.Os!.ToLowerInvariant()}_{toolsPlatform.Cpu!.ToLowerInvariant()}";
    }
}