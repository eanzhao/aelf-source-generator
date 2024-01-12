using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using AElf.SourceGenerator.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AElf.SourceGenerator.Contract;

[Generator]
public class ContractCodeGenerator : IIncrementalGenerator
{
    private ILogger _logger = NullLogger.Instance;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        SetupLogger(context);
        _logger.Log(LogLevel.Debug, "test");
        var protoFiles = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".proto"));

        var protoFilePaths = protoFiles.Select((proto, _) => proto.Path);

        var addedFiles = new List<string>();

        context.RegisterSourceOutput(protoFilePaths, (productionContext, protoFilePath) =>
        {
            //
            var location = Path.GetDirectoryName(protoFilePath);
        });
    }

    private void SetupLogger(
        IncrementalGeneratorInitializationContext context)
    {
        var optionsProvider = context.AnalyzerConfigOptionsProvider.Select((options, _) =>
        {
            var loggingOptions = GetLoggingOptions(options);
            return loggingOptions;
        });
        var logging = optionsProvider
            .Select((options, _) => options)
            .Select((options, _) =>
            {
                _logger = options is null
                    ? NullLogger.Instance
                    : new Logger(options.Value.Level, options.Value.FilePath);
                return 0;
            })
            .SelectMany((_, _) => ImmutableArray<int>.Empty); // don't emit anything

        context.RegisterSourceOutput(logging, static (_, _) =>
        {
            // This delegate will never be called
        });
    }

    private static LoggingOptions? GetLoggingOptions(AnalyzerConfigOptionsProvider options)
    {
        if (!options.GlobalOptions.TryGetValue(ContractSourceGeneratorConstants.LogFilePathProperty,
                out var logFilePath))
            return null;

        if (string.IsNullOrWhiteSpace(logFilePath))
            return null;

        logFilePath = logFilePath.Trim();

        if (!options.GlobalOptions.TryGetValue(ContractSourceGeneratorConstants.LogLevelProperty, out var logLevelValue)
            || !Enum.TryParse(logLevelValue, true, out LogLevel logLevel))
        {
            logLevel = LogLevel.Information;
        }

        return new LoggingOptions(logFilePath, logLevel);
    }
}