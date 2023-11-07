using EventSourcing.Messages;

namespace EventSourcing.Test;
public class LapCompletedSerializerTests
{
    [Fact]
    public void LapCompletedSerializer_ShouldSerailize()
    {
        LapCompletedSerializer serializer = new();

        LapCompleted lap = new()
        {   
            CarNumber = 8, 
            LapNumber = 8, 
            LapTime = TimeSpan.FromSeconds(240)
        };

        byte[] serialized = serializer.Serialize(lap, new Confluent.Kafka.SerializationContext());
        LapCompleted result = serializer.Deserialize(serialized, false, new Confluent.Kafka.SerializationContext());
   
        Assert.Equivalent(lap, result);
    }
}