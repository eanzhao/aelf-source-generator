using AElf;
using Google.Protobuf.Reflection;

namespace ContractGenerator.Primitives;

public static class MessagePrimitives
{

    /// <summary>
    ///     Determines if the proto-message is of EventType based on Aelf.options
    /// </summary>
    public static bool IsEventMessageType(this MessageDescriptor message)
    {
        if (message.GetOptions() == null) return false;
        return message.GetOptions().GetExtension(OptionsExtensions.IsEvent);
    }
}
