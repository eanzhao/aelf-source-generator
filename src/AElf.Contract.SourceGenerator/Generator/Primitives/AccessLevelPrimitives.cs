namespace ContractGenerator.Primitives;

public static class AccessLevelPrimitives
{
    internal static string GetAccessLevel(this GeneratorOptions options)
    {
        return options.InternalAccess ? "internal" : "public";
    }
}
