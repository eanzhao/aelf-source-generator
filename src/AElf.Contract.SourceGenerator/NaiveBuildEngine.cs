using Microsoft.Build.Framework;

namespace AElf.Contract.SourceGenerator;

public class FakeBuildEngine : IBuildEngine
{
    private readonly List<BuildErrorEventArgs> LogErrorEvents = new();

    private readonly List<BuildMessageEventArgs> _logMessageEvents = new();

    private readonly List<CustomBuildEventArgs> _logCustomEvents = new();

    private readonly List<BuildWarningEventArgs> _logWarningEvents = new();

    public bool BuildProjectFile(
        string projectFileName, string[] targetNames,
        System.Collections.IDictionary globalProperties,
        System.Collections.IDictionary targetOutputs)
    {
        return true;
    }

    public int ColumnNumberOfTaskNode => 0;

    public bool ContinueOnError => throw new NotImplementedException();

    public int LineNumberOfTaskNode => 0;

    public void LogCustomEvent(CustomBuildEventArgs e)
    {
        _logCustomEvents.Add(e);
    }

    public void LogErrorEvent(BuildErrorEventArgs e)
    {
        LogErrorEvents.Add(e);
    }

    public void LogMessageEvent(BuildMessageEventArgs e)
    {
        _logMessageEvents.Add(e);
    }

    public void LogWarningEvent(BuildWarningEventArgs e)
    {
        _logWarningEvents.Add(e);
    }

    public string ProjectFileOfTaskNode => "fake ProjectFileOfTaskNode";
}