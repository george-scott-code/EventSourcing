namespace EventSourcing.Events;

public interface IEvent { }

public record RaceEvent(int LapNumber, int CarNumber) : IEvent;

public record LapCompletedEvent(int LapNumber, int CarNumber, string Gap, TimeSpan Time, bool IsDeleted = false) : RaceEvent(LapNumber, CarNumber);

public record LapDeletedEvent(int LapNumber, int CarNumber) : RaceEvent(LapNumber, CarNumber);
