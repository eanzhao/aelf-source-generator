using ContractGenerator.Primitives;
using Google.Protobuf.Reflection;

namespace ContractGenerator;

public class EventTypeGenerator : AbstractGenerator
{
    private readonly MessageDescriptor _messageDescriptor;
    private readonly GeneratorOptions _options;

    public EventTypeGenerator(MessageDescriptor message, GeneratorOptions options) : base(options)
    {
        _messageDescriptor = message;
        _options = options;
    }

    public override string? Generate()
    {
        if (!_messageDescriptor.IsEventMessageType()) return null;
        var accessLevel = _options.GetAccessLevel();
        var typeName = _messageDescriptor.Name;
        PrintLine($"{accessLevel} partial class {typeName} : aelf::IEvent<{typeName}>");
        InBlock(() =>
            {
                GetIndexed();
                ___EmptyLine___();
                GetNonIndexed();
            }
        );
        return PrintOut();
    }

    private void GetIndexed()
    {
        PrintLine(
            $"public global::System.Collections.Generic.IEnumerable<{_messageDescriptor.Name}> GetIndexed()");
        InBlock(() =>
        {
            PrintLine($"return new List<{_messageDescriptor.Name}>");
            InBlockWithSemicolon(() =>
            {
                var fields = _messageDescriptor.Fields.InFieldNumberOrder();
                foreach (var field in fields.Where(f => f.IndexedField()))
                {
                    PrintLine($"new {_messageDescriptor.Name}");
                    InBlockWithComma(() =>
                    {
                        var propertyName = field.GetPropertyName();
                        PrintLine($"{propertyName} = {propertyName}");
                    });
                }
            });
        });
    }

    private void GetNonIndexed()
    {
        PrintLine($"public {_messageDescriptor.Name} GetNonIndexed()");
        InBlock(() =>
        {
            PrintLine($"return new {_messageDescriptor.Name}");
            InBlockWithSemicolon(() =>
            {
                var fields = _messageDescriptor.Fields.InFieldNumberOrder();
                foreach (var field in fields.Where(f => f.NonIndexedField()))
                {
                    var propertyName = field.GetPropertyName();
                    PrintLine($"{propertyName} = {propertyName},");
                }
            });
        });
    }
}
