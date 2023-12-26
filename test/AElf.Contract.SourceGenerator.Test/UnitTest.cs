using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;

namespace AElf.Contract.SourceGenerator.Test;

public class UnitTest
{
    [Fact]
    public void Test()
    {
        var filePath = $"{Environment.CurrentDirectory}/Protobuf/message/transaction_fee.proto";
        var fileText = File.ReadAllText(filePath);
        var output = GetGeneratedOutput(new TestAdditionalFile(filePath, fileText));
        output.Count().ShouldBe(2);
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