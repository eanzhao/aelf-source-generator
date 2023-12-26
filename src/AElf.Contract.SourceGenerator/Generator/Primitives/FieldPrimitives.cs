using AElf;
using Google.Protobuf.Reflection;

namespace ContractGenerator.Primitives;

public static class FieldPrimitives
{
    /// <summary>
    ///     Determines if the proto-message is of IndexedType based on Aelf.options
    /// </summary>
    public static bool IndexedField(this FieldDescriptor field)
    {
        return field.GetOptions() != null && field.GetOptions().GetExtension(OptionsExtensions.IsIndexed);
    }

    public static bool NonIndexedField(this FieldDescriptor field)
    {
        return !field.IndexedField();
    }

    /// <summary>
    ///     This Util gets the PropertyName based on the proto. Copied from the C++ original
    ///     https://github.com/protocolbuffers/protobuf/blob/e57166b65a6d1d55fc7b18beaae000565f617f22/src/google/protobuf/compiler/csharp/csharp_helpers.cc#L255C35-L255C50
    /// </summary>
    public static string GetPropertyName(this FieldDescriptor descriptor)
    {
        var reservedMemberNames = new HashSet<string>
        {
            "Types",
            "Descriptor",
            "Equals",
            "ToString",
            "GetHashCode",
            "WriteTo",
            "Clone",
            "CalculateSize",
            "MergeFrom",
            "OnConstruction",
            "Parser"
        };

        // TODO: consider introducing csharp_property_name field option
        var propertyName = descriptor.GetFieldName().UnderscoresToPascalCase();

        // Avoid either our own type name or reserved names. Note that not all names
        // are reserved - a field called to_string, write_to etc would still cause a problem.
        // There are various ways of ending up with naming collisions, but we try to avoid obvious
        // ones.
        if (propertyName == descriptor.ContainingType.Name
            || reservedMemberNames.Contains(propertyName))
            propertyName += "_";

        return propertyName;
    }

    // Groups are hacky:  The name of the field is just the lower-cased name
    // of the group type.  In C#, though, we would like to retain the original
    // capitalization of the type name.
    public static string GetFieldName(this FieldDescriptor descriptor)
    {
        return descriptor.FieldType == FieldType.Group ? descriptor.MessageType.Name : descriptor.Name;
    }
}
