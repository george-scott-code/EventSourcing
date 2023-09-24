namespace EventSourcing.Events;

public interface IEvent { }
public record LapCompletedEvent(int LapNumber, int CarNumber, string Gap, TimeSpan Time, bool IsDeleted = false) : IEvent;

public record LapDeletedEvent(int LapNumber) : IEvent;
