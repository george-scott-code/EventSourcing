namespace EventSourcing;

public interface IEvent { }
public record LapCompleted(int LapNumber, int CarNumber, string Gap, TimeSpan Time) : IEvent;

public record LapDeleted(int LapNumber) : IEvent;
