namespace EventSourcing;

internal partial class Program
{
    public class RaceEvent
    {
        public Guid Id = Guid.NewGuid();
        public int  CarNumber { get; init; }
    }

}
