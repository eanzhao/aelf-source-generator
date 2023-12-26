using ContractGenerator;

namespace AElf.Contract.SourceGenerator;

public static class ParameterParser
{
    /// <summary>
    ///     Proto Util method based off the C++ original
    ///     https://github.com/protocolbuffers/protobuf/blob/main/src/google/protobuf/compiler/code_generator.cc#L97
    /// </summary>
    internal static List<KeyValuePair<string, string>> ParseGeneratorParameter(string text)
    {
        var output = new List<KeyValuePair<string, string>>();
        var parts = text.Split(',');
        foreach (var part in parts)
        {
            var equalsPos = part.IndexOf('=');
            string key, value;

            if (equalsPos == -1)
            {
                key = part;
                value = string.Empty;
            }
            else
            {
                key = part[..equalsPos];
                value = part[(equalsPos + 1)..];
            }

            output.Add(KeyValuePair.Create(key, value));
        }

        return output;
    }

    internal static GeneratorOptions Parse(string parameter)
    {
        var options = new GeneratorOptions()
        {
            GenerateEvent = true,
            GenerateContract = true
        };
        var pairs = new List<KeyValuePair<string, string>>();
        if (parameter != "")
            pairs = ParseGeneratorParameter(parameter);
        foreach (var (key, _) in pairs)
            switch (key)
            {
                case "stub":
                    options.GenerateStub = true;
                    options.GenerateEvent = true;
                    options.GenerateContract = false;
                    break;
                case "reference":
                    // Reference doesn't require an event
                    options.GenerateReference = true;
                    options.GenerateEvent = false;
                    options.GenerateContract = false;
                    break;
                case "nocontract":
                    options.GenerateContract = false;
                    break;
                case "noevent":
                    options.GenerateEvent = false;
                    break;
                case "internal_access":
                    options.InternalAccess = true;
                    break;
                case "":
                    break;
                default:
                    throw new Exception("Unknown parameter key: " + key);
            }

        return options;
    }

    public static GeneratorOptions Parse(IEnumerable<string> parameters)
    {
        var options = new GeneratorOptions
        {
            GenerateEvent = true,
            GenerateContract = true
        };
        foreach (var parameter in parameters)
        {
            switch (parameter)
            {
                case "stub":
                    options.GenerateStub = true;
                    options.GenerateEvent = true;
                    options.GenerateContract = false;
                    break;
                case "reference":
                    // Reference doesn't require an event
                    options.GenerateReference = true;
                    options.GenerateEvent = false;
                    options.GenerateContract = false;
                    break;
                case "nocontract":
                    options.GenerateContract = false;
                    break;
                case "noevent":
                    options.GenerateEvent = false;
                    break;
                case "internal_access":
                    options.InternalAccess = true;
                    break;
                case "":
                    break;
            }
        }

        return options;
    }
}