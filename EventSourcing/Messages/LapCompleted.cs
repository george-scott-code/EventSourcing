namespace EventSourcing.Messages;

public class LapCompleted : RaceEvent
{
    public TimeSpan LapTime { get; init; }
    public int LapNumber { get; init; }
}