namespace AElf.Contract.SourceGenerator.Extensions;

public static class StringExtensions
{
    public static string Print(this IEnumerable<string>? strings)
    {
        return strings == null ? string.Empty : strings.Aggregate(string.Empty, (a, b) => $"{a}\n{b}");
    }
}