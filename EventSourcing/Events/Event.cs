namespace EventSourcing;

public interface IEvent { }
public record LapCompleted(int LapNumber, int CarNumber, string Gap, TimeSpan Time, bool IsDeleted = false) : IEvent;

public record LapDeleted(int LapNumber) : IEvent;
