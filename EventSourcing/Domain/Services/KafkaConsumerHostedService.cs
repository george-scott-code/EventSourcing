using Confluent.Kafka;
using EventSourcing.Infrastructure;
using EventSourcing.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Domain.Services;
// TODO: decouple service
public class KafkaConsumerHostedService : IHostedService, IDisposable
{
    private readonly ITimingRepository _timingRepository;
    private readonly ILogger<KafkaConsumerHostedService> _logger;
    private readonly ConsumerConfig _config;
    private readonly IConsumer<Null, LapCompleted> _consumer;
    private bool _cancelled;

    public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger, ITimingRepository timingRepository)
    {
        _timingRepository = timingRepository;
        _logger = logger;
        //TODO: extract / inject
        _config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:29092",
            GroupId = "foo",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<Null, LapCompleted>(_config)
            .SetValueDeserializer(new LapCompletedSerializer())
            .SetErrorHandler((_, error) => _logger.LogError(error.ToString()))
            .Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe("demo");
        while (!cancellationToken.IsCancellationRequested && !_cancelled)
        {
            var consumeResult = _consumer.Consume(cancellationToken);
            LapCompleted raceEvent = consumeResult.Message.Value;
            ProcessRaceEvent(raceEvent);
        }
        _consumer.Close();
        
        return Task.CompletedTask;
    }

    private void ProcessRaceEvent(LapCompleted raceEvent)
    {
        switch (raceEvent)
        {
            case LapCompleted lapCompleted:
            {
                _logger.LogInformation($"{lapCompleted.CarNumber} - {lapCompleted.LapTime}");
                CarTiming car = _timingRepository.Get(lapCompleted.CarNumber);
                car.LapCompleted(lapCompleted.LapNumber, "-", lapCompleted.LapTime);
                _timingRepository.Save(car);
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
    
    public void Dispose()
    {
        _ = StopAsync(new CancellationToken());
        _cancelled = true;
        _consumer?.Close();
        _consumer?.Dispose();
    }
}
