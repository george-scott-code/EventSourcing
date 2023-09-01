namespace EventSourcing;

public interface IEvent { }
public record LapCompleted(int CarNumber, string Gap, string Time) : IEvent;
