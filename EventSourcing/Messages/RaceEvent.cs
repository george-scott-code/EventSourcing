namespace EventSourcing.Messages;

    public class RaceEvent
    {
        public Guid Id = Guid.NewGuid();
        public int  CarNumber { get; init; }
    }


