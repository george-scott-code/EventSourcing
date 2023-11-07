using System.Text.Json;
using Confluent.Kafka;

namespace EventSourcing.Messages;

public class LapCompletedSerializer : ISerializer<LapCompleted>, IDeserializer<LapCompleted>
{
    public byte[] Serialize(LapCompleted data, SerializationContext context)
    {
        using var ms = new MemoryStream();

        string jsonString = JsonSerializer.Serialize(data);
        var writer = new StreamWriter(ms);
        writer.Write(jsonString);
        writer.Flush();
        ms.Position = 0;
        return ms.ToArray();
    }
    
    public LapCompleted Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<LapCompleted>(data.ToArray()) ?? new LapCompleted();
    }
}