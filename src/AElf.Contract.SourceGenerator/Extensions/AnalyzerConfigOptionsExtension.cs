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

    public static GenerateType GetGenerateType(this AnalyzerConfigOptions options)
    {
        var key = "build_metadata.additionalfiles.generatetype";
        options.TryGetValue(key, out var generateType);
        Enum.TryParse(generateType, true, out GenerateType gt);
        return gt;
    }
}