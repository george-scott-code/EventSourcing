using EventSourcing.Messages;

namespace EventSourcing.Test;
public class LapCompletedSerializerTests
{
    [Fact]
    public void LapCompletedSerializer_ShouldSerailize()
    {
        var target = new LapCompletedSerializer();

        LapCompleted lap = new()
        {   
            CarNumber = 8, 
            LapNumber = 8, 
            LapTime = TimeSpan.FromSeconds(240)
        };

        var serialized = target.Serialize(lap, new Confluent.Kafka.SerializationContext());
        var result = target.Deserialize(serialized, false, new Confluent.Kafka.SerializationContext());
   
        Assert.Equivalent(lap, result);
    }
}