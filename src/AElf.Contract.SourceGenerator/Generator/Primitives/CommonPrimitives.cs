using System.Text;

namespace ContractGenerator.Primitives;

public static class CommonPrimitives
{
    internal static string UnderscoresToPascalCase(this string input)
    {
        return UnderscoresToCamelCase(input, true);
    }

    /// <summary>
    ///     This Util does more than just convert underscores to camel-case. copied from the C++ original
    ///     https://github.com/protocolbuffers/protobuf/blob/e57166b65a6d1d55fc7b18beaae000565f617f22/src/google/protobuf/compiler/csharp/names.cc#L138
    /// </summary>
    internal static string UnderscoresToCamelCase(this string input, bool capNextLetter, bool preservePeriod = false)
    {
        var result = "";
        for (var i = 0; i < input.Length; i++)
            switch (input[i])
            {
                case >= 'a' and <= 'z':
                {
                    if (capNextLetter)
                        result += (char)(input[i] + ('A' - 'a'));
                    else
                        result += input[i];
                    capNextLetter = false;
                    break;
                }
                case >= 'A' and <= 'Z':
                {
                    if (i == 0 && !capNextLetter)
                        // Force first letter to lower-case unless explicitly told to
                        // capitalize it.
                        result += (char)(input[i] + ('a' - 'A'));
                    else
                        // Capital letters after the first are left as-is.
                        result += input[i];
                    capNextLetter = false;
                    break;
                }
                case >= '0' and <= '9':
                    result += input[i];
                    capNextLetter = true;
                    break;
                default:
                {
                    capNextLetter = true;
                    if (input[i] == '.' && preservePeriod) result += '.';

                    break;
                }
            }

        // Add a trailing "_" if the name should be altered.
        if (input.Length > 0 && input[^1] == '#') result += '_';

        // https://github.com/protocolbuffers/protobuf/issues/8101
        // To avoid generating invalid identifiers - if the input string
        // starts with _<digit> (or multiple underscores then digit) then
        // we need to preserve the underscore as an identifier cannot start
        // with a digit.
        // This check is being done after the loop rather than before
        // to handle the case where there are multiple underscores before the
        // first digit. We let them all be consumed so we can see if we would
        // start with a digit.
        // Note: not preserving leading underscores for all otherwise valid identifiers
        // so as to not break anything that relies on the existing behaviour
        if (result.Length > 0 && '0' <= result[0] && result[0] <= '9'
            && input.Length > 0 && input[0] == '_')
            // Concatenate a _ at the beginning
            result = '_' + result;
        return result;
    }

    internal static string LowerUnderscoreToUpperCamel(this string str)
    {
        var tokens = str.Split('_');
        var result = new StringBuilder();

        foreach (var token in tokens) result.Append(token.CapitalizeFirstLetter());

        return result.ToString();
    }

    internal static string CapitalizeFirstLetter(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;

        var chars = str.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return new string(chars);
    }
}
