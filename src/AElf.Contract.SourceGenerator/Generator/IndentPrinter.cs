using System.Text;

namespace ContractGenerator;

public class IndentPrinter
{
    private readonly StringBuilder _stringBuilder = new();
    private int _indents;

    public void Indent()
    {
        _indents++;
    }

    public void Outdent()
    {
        if (_indents == 0) throw new Exception("nothing left to outdent");

        _indents--;
    }

    public void ___EmptyLine___()
    {
        PrintLine(String.Empty);
    }

    public void Print(string str)
    {
        if (string.IsNullOrEmpty(str)) return;
        var lines = str.Split(Environment.NewLine);
        foreach (var line in lines.SkipLast(1))
        {
            PrintOneLine(line);
        }

        _stringBuilder.Append(lines.Last());
    }

    public void PrintIgnoreWhitespace(string? str)
    {
        if (string.IsNullOrWhiteSpace(str)) return;
        Print(str);
    }

    public void PrintLine(string str)
    {
        var lines = str.Split(Environment.NewLine);
        foreach (var line in lines)
        {
            PrintOneLine(line);
        }
    }

    public string PrintOut()
    {
        return _stringBuilder.ToString();
    }

    private void PrintOneLine(string line)
    {
        if (line.Trim().Length == 0)
        {
            _stringBuilder.AppendLine();
            return;
        }

        for (var i = 0; i < _indents; i++) _stringBuilder.Append("  ");
        _stringBuilder.AppendLine(line);
    }
}
