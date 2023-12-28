using AElf.Contract.SourceGenerator.Logging;
using Microsoft.CodeAnalysis;

namespace AElf.Contract.SourceGenerator;

//[Generator]
public class ProtobufCodeGenerator : IIncrementalGenerator
{
    private ILogger _logger = NullLogger.Instance;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var protoFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".proto"));
    }
}