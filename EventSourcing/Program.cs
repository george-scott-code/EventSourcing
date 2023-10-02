using EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventSourcing.Domain.Services;

namespace EventSourcing;

internal partial class Program
{
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, collection) =>
            {
                collection.AddHostedService<KafkaProducerHostedService>();
                collection.AddHostedService<KafkaConsumerHostedService>();
                collection.AddSingleton<ITimingRepository, TimingRepository>();
            });

    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.RunAsync();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var timingRepository = services.GetRequiredService<ITimingRepository>();

        char input = 'x';

        System.Console.WriteLine("Press 'q' to quit");
        System.Console.WriteLine("Press 'f' to get fastest laps");

        while(input != 'q')
        {
            input = Console.ReadKey(true).KeyChar;

            switch(input)
            {
                case 'f':
                    PrintFastestLapTimes(timingRepository);
                    break;
            }
        }

        // // TODO: Load Data from file
        // // TODO: Persistent Storage
        // // TODO: Data from events, try apache kafka?
        // // TODO: Projections
        // // TODO: Tests when patterns established
        // // TODO: do we want to update per lap time or per lap? 
        // //       both "race" and drivers could subscribe to events



        // // TODO: interactive input / run lap by lap
        // // TODO: tests
        // car.LapCompleted(1, "8.797", TimeSpan.ParseExact("01:44.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
        // car.LapCompleted(2, "8.797", TimeSpan.ParseExact("01:45.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
        // car.LapCompleted(3, "8.797", TimeSpan.ParseExact("01:43.788", @"mm\:ss\.fff", CultureInfo.InvariantCulture, TimeSpanStyles.None));
        // car.DeleteLapTime(3);
    
    }

    private static void PrintFastestLapTimes(ITimingRepository timingRepository)
    {
        //TODO: should be loading projection data when available for efficiency
        var timings = timingRepository.Get();

        foreach (var timing in timings)
        {
            Console.WriteLine($"Car {timing.CarNumber} has completed {timing.GetLapsCompleted()} laps");
            Console.WriteLine($"PB: {timing.GetFastestLap()}");
        }
    }
}
