namespace ContractGenerator;

public abstract class AbstractGenerator : IndentPrinter
{
    protected readonly GeneratorOptions Options;

    protected AbstractGenerator(GeneratorOptions options)
    {
        Options = options;
    }

    protected void InRegion(string name, Action a)
    {
        PrintLine($"#region {name}");
        a();
        PrintLine($"#endregion {name}");
    }

    protected void InBlock(Action a)
    {
        PrintLine("{");
        Indent();
        a();
        Outdent();
        PrintLine("}");
    }

    protected void InBlockWithSemicolon(Action a)
    {
        PrintLine("{");
        Indent();
        a();
        Outdent();
        PrintLine("};");
    }

    protected void InBlockWithComma(Action a)
    {
        PrintLine("{");
        Indent();
        a();
        Outdent();
        PrintLine("},");
    }

    public abstract string? Generate();
}
