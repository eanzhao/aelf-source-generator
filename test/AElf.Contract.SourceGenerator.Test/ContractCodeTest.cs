using System.Reflection;
using Google.Protobuf;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using Shouldly;

namespace AElf.Contract.SourceGenerator.Test;

public class ContractCodeTest
{
    [Fact]
    public void Test()
    {
        var base64Code = File.ReadAllText("code.txt");
        var stream = File.OpenRead("code.txt");
        var code = ByteString.FromBase64(base64Code);
        var str = code.ToString();
        var assembly = Assembly.Load(code.ToByteArray());
        var peFile = new PEFile(assembly.ManifestModule.ScopeName, stream);
        //var resolver = new UniversalAssemblyResolver(assembly.FullName, true);
        //var decompiler = new CSharpDecompiler(new DecompilerTypeSystem(new PEFile()));
        using var writer = new StringWriter();
        var metadata = peFile.Metadata;
        var output = new PlainTextOutput(writer);
        var rd = new ReflectionDisassembler(output, CancellationToken.None)
        {
            DetectControlStructure = false
        };
        rd.WriteAssemblyReferences(peFile.Metadata);
        if (metadata.IsAssembly)
            rd.WriteAssemblyHeader(peFile);
        output.WriteLine();
        rd.WriteModuleHeader(peFile);
        output.WriteLine();
        rd.WriteModuleContents(peFile);

        var result = writer.ToString();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Test2()
    {
        var base64Code = await File.ReadAllTextAsync("code.txt");
        var byteArray = Convert.FromBase64String(base64Code);
        await using var fs = new FileStream("AElf.Contract.dll", FileMode.Create, FileAccess.Write);
        await fs.WriteAsync(byteArray);
        var module = new PEFile("AElf.Contract.dll");
        var settings = new DecompilerSettings(LanguageVersion.Latest)
        {
            ThrowOnAssemblyResolveErrors = false,
            RemoveDeadCode = false,
            RemoveDeadStores = false
        };
        var resolver =
            new UniversalAssemblyResolver("AElf.Contract.dll", false, module.Metadata.DetectTargetFrameworkId());
        var decompiler = new WholeProjectDecompiler(settings, resolver, resolver, null);
        const string dir = "./AElf.Contract";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        decompiler.DecompileProject(module, dir);
        module.Reader.Dispose();
    }

    [Fact]
    public void Test3()
    {
        //var location = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        var location = Assembly.GetExecutingAssembly().Location;
    }
}