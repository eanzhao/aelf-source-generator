namespace AElf.Contract.SourceGenerator.Generator.Primitives;

public static class AccessLevelPrimitives
{
    internal static string GetAccessLevel(this GeneratorOptions options)
    {
        return options.InternalAccess ? "internal" : "public";
    }
}
