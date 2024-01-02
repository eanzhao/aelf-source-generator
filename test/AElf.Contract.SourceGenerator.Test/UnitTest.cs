using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;

namespace AElf.Contract.SourceGenerator.Test;

public class UnitTest
{
    [Theory]
    [InlineData("message/authority_info.proto")]
    [InlineData("message/transaction_fee.proto")]
    public void Test(string file)
    {
        var filePath = $"{Environment.CurrentDirectory}/Protobuf/Proto/{file}";
        var fileText = File.ReadAllText(filePath);
        var output = GetGeneratedOutput(new TestAdditionalFile(filePath, fileText));
    }

    private static IEnumerable<(string, string)> GetGeneratedOutput(params AdditionalText[] additionalFiles)
    {
        var compilation = CSharpCompilation.Create("ContractCodeGeneratorTests");
        var generator = new ContractCodeGenerator();
        CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(additionalFiles.ToImmutableArray())
            .RunGeneratorsAndUpdateCompilation(compilation,
                out var outputCompilation,
                out var diagnostics);

        // optional
        diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();

        return outputCompilation.SyntaxTrees.Select(t => (t.FilePath.ToString(), t.GetText().ToString()));
    }
}