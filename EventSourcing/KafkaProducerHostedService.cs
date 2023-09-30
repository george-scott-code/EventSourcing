using System.Globalization;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EventSourcing.Messages;

namespace EventSourcing;

internal partial class Program
{
    // TODO: decouple service
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
                .SetValueSerializer(new LapCompletedSerializer())
                .SetErrorHandler((_, error) => _logger.LogError(error.ToString()))
                .Build();

        }

         public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Data\Lap1.txt");
                string[] lines = File.ReadAllLines(path);

                foreach(var line in lines.Skip(1))
                {
                    string[] data = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var lap = new LapCompleted()
                    {
                        LapNumber = 1,
                        CarNumber = int.Parse(data[0]),
                        // TODO: gap
                        // TODO: simplify inititialization
                        LapTime = TimeSpan.ParseExact(data[2], @"m\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None)
                    };

                    await _producer.ProduceAsync("demo", new Message<Null, LapCompleted>()
                    {
                        Value = lap
                    }, cancellationToken);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }

}
