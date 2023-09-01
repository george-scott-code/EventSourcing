namespace EventSourcing;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        // Temp:
        var timingRepository = new TimingRepository();
        // TODO: Load Data from file
        // TODO: Persistent Storage
        // TODO: Data from events
        // TODO: Projections
        // TODO: do we want to update per lap time or per lap?

        //  LAP 1   GAP     TIME
        //  11              1:35.991
        //  16      1.097   1:37.088
        //  1       2.091   1:38.082
        //  55      2.644   1:38.635
        //  31      3.909   1:39.900
        //  63      4.430   1:40.421
        //  14      5.467   1:41.458
        //  77      6.132   1:42.123
        //  20      6.714   1:42.705
        //  10      7.115   1:43.106
        //  4       7.342   1:43.333
        //  18      7.837   1:43.828
        //  3       8.283   1:44.274
        //  44      8.797   1:44.788
        //  23      9.032   1:45.023
        //  27      9.812   1:45.803
        //  6       10.386  1:46.377
        //  24      10.529  1:46.520

        var car = timingRepository.Get(44);

        // TODO: interactive input / run lap by lap

        car.LapCompleted("8.797", "1:44.788");

        Console.WriteLine($"Car {car.CarNumber} has completed {car.GetLapsCompleted()} laps");
    }
}
