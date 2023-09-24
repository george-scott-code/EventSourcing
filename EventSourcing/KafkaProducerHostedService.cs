using System.Globalization;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing;

internal partial class Program
{
    public class KafkaProducerHostedService : IHostedService
    {
        private readonly ILogger<KafkaProducerHostedService> _logger;
        private IProducer<Null, LapCompleted> _producer;

        public KafkaProducerHostedService(ILogger<KafkaProducerHostedService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            // TODO: support generic race events
            _producer = new ProducerBuilder<Null, LapCompleted>(config)
                .SetValueSerializer(new LapCompleted())
                .Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // car.LapCompleted(1, "8.797", TimeSpan.ParseExact("01:44.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
            // car.LapCompleted(2, "8.797", TimeSpan.ParseExact("01:45.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
            // car.LapCompleted(3, "8.797", TimeSpan.ParseExact("01:43.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
            // TODO: extract and pass from collection / parse
            await _producer.ProduceAsync("demo", new Message<Null, LapCompleted>()
            {
                Value = new LapCompleted()
                {
                    LapNumber = 1,
                    CarNumber = 44,
                    // TODO: simplify inititialization
                    LapTime = TimeSpan.ParseExact("01:44.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None)
                }
            }, cancellationToken);

            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }
    
    public class RaceEvent
    {
        public Guid Id = Guid.NewGuid();
        public int  CarNumber { get; init; }
    }

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

}
