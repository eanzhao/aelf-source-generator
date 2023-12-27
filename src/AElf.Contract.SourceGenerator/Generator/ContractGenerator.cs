using AElf.Contract.SourceGenerator.Generator.Primitives;
using Google.Protobuf.Compiler;
using Google.Protobuf.Reflection;

namespace AElf.Contract.SourceGenerator.Generator;

//This is the main entry-point into this project and is exposed to external users
public class ContractGenerator
{
    /// <summary>
    ///     Generates a set of C# files from the input stream containing the proto source. This is the primary entry-point into
    ///     the ContractPlugin.
    /// </summary>
    public IReadOnlyList<CodeGeneratorResponse.Types.File> Generate(IEnumerable<FileDescriptor> fileDescriptors,
        GeneratorOptions options)
    {
        var output = new List<CodeGeneratorResponse.Types.File>();

        var descriptors = fileDescriptors as FileDescriptor[] ?? fileDescriptors.ToArray();
        foreach (var file in descriptors)
        {
            var generatedCsCodeBody = new ServiceGenerator(file, options).Generate() ?? "";
            if (generatedCsCodeBody.Length == 0)
                // don't generate a file if there are no services
                continue;

            // Get output file name.
            var fileName = file.GetOutputCSharpFilename();
            if (fileName == "") return output;

            // Only generate specific proto files, don't generate "import proto".
            if (file == descriptors.Last())
            {
                output.Add(
                    new CodeGeneratorResponse.Types.File
                    {
                        Name = fileName,
                        Content = generatedCsCodeBody
                    }
                );
            }
        }

        return output;
    }
}