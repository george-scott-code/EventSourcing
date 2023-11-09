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
        IHost host = CreateHostBuilder(args).Build();
        host.RunAsync();

        using IServiceScope? scope = host.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        var timingRepository = services.GetRequiredService<ITimingRepository>();

        char input = default;

        PrintOptions();

        while(input != 'q')
        {
            input = Console.ReadKey(true).KeyChar;

            switch (input)
            {
                case 'f':
                    PrintFastestLapTimes(timingRepository);
                    break;
                case 'l':
                    Console.WriteLine("enter lap number");
                    var lapInput = Console.ReadLine();
                    _ = int.TryParse(lapInput, out int lap);
                    PrintFastestLapTimes(timingRepository, lap);
                    break;
                default:
                    PrintOptions();
                    break;
            }
        }

        // TODO: Load Data from file
        // TODO: Persistent Storage
        // TODO: Data from events, try apache kafka?
        // TODO: Projections
        // TODO: Tests when patterns established
        // TODO: do we want to update per lap time or per lap? 
        //       both "race" and drivers could subscribe to events
        // TODO: interactive input / run lap by lap
        // TODO: retry / unique key
    }

    private static void PrintOptions()
    {  
        Console.WriteLine("Press 'q' to quit");
        Console.WriteLine("Press 'f' to get fastest laps");
        Console.WriteLine("Press 'l' to get fastest laps to target lap");
    }


    private static void PrintFastestLapTimes(ITimingRepository timingRepository, int? lap = null)
    {
        IList<CarTiming> timings;
        //TODO: should be loading projection data when available for efficiency
        if (lap is null)
        {
            timings = timingRepository.Get();
        }
        else
        {
            timings = timingRepository.GetToLap(lap.Value);
        }

        foreach (CarTiming timing in timings)
        {
            Console.WriteLine($"Car {timing.CarNumber} has completed {timing.GetLapsCompleted()} laps");
            Console.WriteLine($"PB: {timing.GetFastestLap()}");
        }
    }
}
