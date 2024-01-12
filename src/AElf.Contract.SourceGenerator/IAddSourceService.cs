using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AElf.Contract.SourceGenerator;

public interface IAddSourceService
{
    void AddSource(SourceProductionContext spc, string name, string text, string protoFilePath);
    void Perform();
}

// TODO: Remove existing files firstly
public class AddSourceToGeneratedDirectoryService : IAddSourceService
{
    public void AddSource(SourceProductionContext spc, string name, string text, string protoFilePath)
    {
        // TODO: Add sources to a cache.

        var path = $"{Path.GetDirectoryName(protoFilePath)}{Path.DirectorySeparatorChar}{name}";
        if (!path.EndsWith(".cs"))
        {
            path += ".cs";
        }

        path = path.Replace($"{Path.DirectorySeparatorChar}Proto{Path.DirectorySeparatorChar}",
            $"{Path.DirectorySeparatorChar}Generated{Path.DirectorySeparatorChar}");
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        }

        File.WriteAllText(path, text);
    }

    public void Perform()
    {
        throw new NotImplementedException();
    }
}

public class AddSourceService : IAddSourceService
{
    public void AddSource(SourceProductionContext spc, string name, string text, string protoFilePath)
    {
        spc.AddSource(name, SourceText.From(text, Encoding.UTF8));
    }

    public void Perform()
    {
        throw new NotImplementedException();
    }
}