using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing;

internal partial class Program
{
    public class KafkaConsumerHostedService : IHostedService
    {
        private ILogger<KafkaConsumerHostedService> _logger;
        private ConsumerConfig _config;
        private IConsumer<Null, LapCompleted?> _consumer;
        private bool _cancelled;

        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger)
        {
            _logger = logger;

            _config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, LapCompleted?>(_config)
                .SetValueDeserializer(new LapCompleted())
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: exit gracefully
            // using (var consumer = new ConsumerBuilder<Ignore, string>(_config).Build())
            // {
                _consumer.Subscribe("demo");

                while (!_cancelled)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    LapCompleted? raceEvent = consumeResult.Message.Value;
                    ProcessRaceEvent(raceEvent);
                }

                _consumer.Close();
            // }

            return Task.CompletedTask;
        }

        private void ProcessRaceEvent(LapCompleted? raceEvent)
        {
            switch (raceEvent)
            {
                case LapCompleted lapCompleted:
                {
                    _logger.LogInformation($"{lapCompleted.CarNumber} - {lapCompleted.LapTime}");
                    return;
                }
                default:
                {
                    _logger.LogInformation("could not parse event");
                    return;
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancelled = true;
            _consumer?.Close();
            _consumer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
