// using Microsoft.CodeAnalysis;
//
// namespace AElf.Contract.SourceGenerator;
//
// [Generator]
// public class ContractStateGenerator : IIncrementalGenerator
// {
//     public void Initialize(IncrementalGeneratorInitializationContext context)
//     {
//         var stateFiles = context.AdditionalTextsProvider
//             .Where(static file => file.Path.EndsWith(".state"));
//
//         var stateFilePaths = stateFiles.Select((proto, _) => proto.Path);
//
//         context.RegisterSourceOutput(stateFilePaths, (productionContext, stateFilePath) =>
//         {
//
//         }
//     }
// }