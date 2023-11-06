using System.Globalization;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EventSourcing.Messages;

namespace EventSourcing.Domain.Services;
// TODO: decouple service
public class KafkaProducerHostedService : IHostedService
{
    private readonly ILogger<KafkaProducerHostedService> _logger;
    private readonly IProducer<Null, LapCompleted> _producer;
    private readonly ICollection<LapCompleted> laps = new List<LapCompleted>();

    public KafkaProducerHostedService(ILogger<KafkaProducerHostedService> logger)
    {
        _logger = logger;
        var config = new ProducerConfig()
        {
            BootstrapServers = "localhost:29092"
        };
        // TODO: support generic race events
        _producer = new ProducerBuilder<Null, LapCompleted>(config)
            .SetValueSerializer(new LapCompletedSerializer())
            .SetErrorHandler((_, error) => _logger.LogError(error.ToString()))
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach(LapCompleted lap in ParseLapTimes())
        {
            await _producer.ProduceAsync("demo", new Message<Null, LapCompleted>()
            {
                Value = lap
            }, cancellationToken);
        }
        _producer.Flush(TimeSpan.FromSeconds(10));
    }

    private IEnumerable<LapCompleted> ParseLapTimes()
    {
        var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        try
        {
            string path = Path.Combine(directory, @"Data\Lap1.txt");
            string[] lines = File.ReadAllLines(path);
            ParseLapTime(1, lines);

            string path2 = Path.Combine(directory, @"Data\Lap2.txt");
            string[] lines2 = File.ReadAllLines(path2);
            ParseLapTime(2, lines2);
        }
        //TODO: handle file exception and parsing exceptions separately
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
        return laps;
    }

    private void ParseLapTime(int lapNumber, string[] lines)
    {
        foreach(var line in lines.Skip(1))
        {
            string[] data = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            laps.Add( new LapCompleted()
            {
                LapNumber = lapNumber,
                CarNumber = int.Parse(data[0]),
                LapTime = TimeSpan.ParseExact(data[2], @"m\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None)
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _producer?.Dispose();
        return Task.CompletedTask;
    }
}
