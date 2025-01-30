using System.Text;
using Core.Models;

namespace Core.Serialization;

// TODO: public
internal class Serializer
{
    public static string Serialize(List<StartBlock> startBlocks)
    {
        var sb = new StringBuilder();
        foreach (var block in startBlocks)
        {
            var idVisitor = new IdVisitor();
            block.Accept(idVisitor);
            var serializationVisitor = new SerializationVisitor(idVisitor.Ids);
            block.Accept(serializationVisitor);
            sb.AppendLine(serializationVisitor.String);
        }

        return sb.ToString().Trim();
    }

    public static List<StartBlock> Deserialize(string serialized)
    {
        var parts = serialized.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return parts
            .Select(static p => new BlockDeserializer().Deserialize(p))
            .ToList();
    }
}