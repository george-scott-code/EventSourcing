using System.Globalization;
using System.Text;
using Confluent.Kafka;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing;

internal class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, collection) =>
            {
                collection.AddHostedService<KafkaConsumerHostedService>();
                collection.AddHostedService<KafkaProducerHostedService>();
            });

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        CreateHostBuilder(args).Build().Run();
        // var timingRepository = new TimingRepository();
        // // TODO: Load Data from file
        // // TODO: Persistent Storage
        // // TODO: Data from events, try apache kafka?
        // // TODO: Projections
        // // TODO: Tests when patterns established
        // // TODO: do we want to update per lap time or per lap? 
        // //       both "race" and drivers could subscribe to events

        // //  LAP 1   GAP     TIME
        // //  11              1:35.991
        // //  16      1.097   1:37.088
        // //  1       2.091   1:38.082
        // //  55      2.644   1:38.635
        // //  31      3.909   1:39.900
        // //  63      4.430   1:40.421
        // //  14      5.467   1:41.458
        // //  77      6.132   1:42.123
        // //  20      6.714   1:42.705
        // //  10      7.115   1:43.106
        // //  4       7.342   1:43.333
        // //  18      7.837   1:43.828
        // //  3       8.283   1:44.274
        // //  44      8.797   1:44.788
        // //  23      9.032   1:45.023
        // //  27      9.812   1:45.803
        // //  6       10.386  1:46.377
        // //  24      10.529  1:46.520

        // var car = timingRepository.Get(44);

        // // TODO: interactive input / run lap by lap
        // // TODO: tests
        // car.LapCompleted(1, "8.797", TimeSpan.ParseExact("01:44.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
        // car.LapCompleted(2, "8.797", TimeSpan.ParseExact("01:45.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
        // car.LapCompleted(3, "8.797", TimeSpan.ParseExact("01:43.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
        // car.DeleteLapTime(3);
        
        // // persist changes
        // timingRepository.Save(car);

        // // TODO: should be loading projection data when available for efficiency
        // var timings = timingRepository.Get();

        // foreach (var timing in timings)
        // {
        //     Console.WriteLine($"Car {timing.CarNumber} has completed {timing.GetLapsCompleted()} laps");
        //     Console.WriteLine($"PB: {timing.GetFastestLap()}");
        // }
    }

    public class KafkaProducerHostedService : IHostedService
    {
        private readonly ILogger<KafkaProducerHostedService> _logger;
        private IProducer<Null, string> _producer;

        public KafkaProducerHostedService(ILogger<KafkaProducerHostedService> logger)
        {
            _logger = logger;
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for(var i = 0; i < 10; ++i)
            {
                var value = $"Hello World {i}";
                _logger.LogInformation(value);
                await _producer.ProduceAsync("demo", new Message<Null, string>()
                {
                    Value = value
                }, cancellationToken);
            }

            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }

    public class KafkaConsumerHostedService : IHostedService
    {
        private ILogger<KafkaConsumerHostedService> _logger;
        private ClusterClient _cluster;

        public KafkaConsumerHostedService(ILogger<KafkaConsumerHostedService> logger)
        {
            _logger = logger;

            // TODO: try using Kafka Package
            _cluster = new ClusterClient(new Configuration
            {
                Seeds = "localhost:9092"
            }, new ConsoleLogger());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest("demo");
            _cluster.MessageReceived += record => 
            {
                // TODO: serialisation
                _logger.LogInformation($"Recieved: {Encoding.UTF8.GetString(record.Value as byte[])}");
            };

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cluster?.Dispose();
            return Task.CompletedTask;
        }

    }

}

//consumer:
// https://youtu.be/n_IQq3pze0s?t=687
