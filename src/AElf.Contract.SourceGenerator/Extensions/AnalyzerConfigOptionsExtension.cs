using Microsoft.CodeAnalysis.Diagnostics;

namespace AElf.Contract.SourceGenerator.Extensions;

public static class AnalyzerConfigOptionsExtensions
{
    public static string Print(this AnalyzerConfigOptions options)
    {
        var output = string.Empty;
        var keys = options.Keys;
        foreach (var key in keys)
        {
            if (options.TryGetValue(key, out var value))
            {
                output += $"{key}: {value}\n";
            }
            else
            {
                output += key;
            }
        }

        return output;
    }

    public static string? GetContractOutputOptions(this AnalyzerConfigOptions options)
    {
        const string key = "build_metadata.additionalfiles.contractoutputoptions";
        options.TryGetValue(key, out var value);
        return value;
    }

    public static string? GetAccess(this AnalyzerConfigOptions options)
    {
        const string key = "build_metadata.additionalfiles.access";
        options.TryGetValue(key, out var value);
        return value;
    }
}