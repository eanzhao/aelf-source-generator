namespace AElf.SourceGenerator.Contract;

public class NamespaceProvider
{
    private readonly string _namespace;

    public NamespaceProvider(string ns)
    {
        _namespace = ns;
    }

    public string GetNamespace()
    {
        return _namespace;
    }
}