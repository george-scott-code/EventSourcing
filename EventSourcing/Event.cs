namespace EventSourcing;

public interface IEvent { }
public record LapCompleted(int CarNumber, string Gap, TimeSpan Time) : IEvent;
