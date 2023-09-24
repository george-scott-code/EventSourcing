using System.Text.Json;
using Confluent.Kafka;

namespace EventSourcing.Messages;

public class LapCompleted : RaceEvent, ISerializer<LapCompleted>, IDeserializer<LapCompleted?>
{
    public TimeSpan LapTime { get; init; }
    public int LapNumber { get; init; }
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
    // TODO why no access modifier here?
    LapCompleted? IDeserializer<LapCompleted?>.Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<LapCompleted>(data.ToArray());
    }
}