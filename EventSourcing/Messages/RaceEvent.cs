namespace EventSourcing.Messages;

public class RaceEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int CarNumber { get; init; }
}