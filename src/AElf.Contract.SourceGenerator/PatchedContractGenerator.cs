using AElf.CSharp.CodeOps;
using AElf.CSharp.CodeOps.Instructions;
using AElf.CSharp.CodeOps.Patchers;
using AElf.CSharp.CodeOps.Patchers.Module;
using AElf.CSharp.CodeOps.Patchers.Module.SafeMath;
using AElf.CSharp.CodeOps.Patchers.Module.SafeMethods;
using AElf.CSharp.CodeOps.Policies;
using AElf.CSharp.CodeOps.Validators;
using AElf.CSharp.CodeOps.Validators.Assembly;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;

namespace AElf.Contract.SourceGenerator;

// [Generator(LanguageNames.CSharp)]
public class PatchedContractGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var compilations = context.CompilationProvider.Select((compilation, _) => compilation);
        context.RegisterSourceOutput(compilations, (productionContext, compilation) =>
        {
            var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
            if (syntaxTree == null)
            {
                return;
            }

            var root = syntaxTree.GetRoot(productionContext.CancellationToken);



            var classDeclarationSyntax = root
                .DescendantNodes(descendIntoTrivia: true)
                .OfType<ClassDeclarationSyntax>();

            var patcher = new CSharpContractPatcher(new DefaultPolicy(new IValidator[] { },
                new IPatcher<ModuleDefinition>[]
                {
                    new StateWrittenSizeLimitMethodInjector(new StateWrittenInstructionInjector()),
                    new ResetFieldsMethodInjector(),
                    new StringMethodsReplacer(),
                    new Patcher(),
                    new CSharp.CodeOps.Patchers.Module.CallAndBranchCounts.Patcher()
                }));
            //patcher.Patch()
            //productionContext.AddSource("test", source);
        });
    }
}