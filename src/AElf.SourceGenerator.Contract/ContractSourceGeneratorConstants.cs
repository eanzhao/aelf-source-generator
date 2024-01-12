namespace AElf.SourceGenerator.Contract;

public class ContractSourceGeneratorConstants
{
    public const string LogFileName = "aelf-source-generator-contract.log";
    private const string PropertyPrefix = "build_property.";
    public const string LogFilePathProperty = $"{PropertyPrefix}AElfSourceGenerator_LogFilePath";
    public const string LogLevelProperty = $"{PropertyPrefix}AElfSourceGenerator_LogLevel";
}