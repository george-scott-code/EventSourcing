
namespace EventSourcing;

public class CurrentState
{
    public Dictionary<int, LapTiming> LapsCompleted { get; set; } = new Dictionary<int, LapTiming>();
}

public class LapTiming
{
    public LapTiming(TimeSpan time)
    {
        Time = time;
    }

    public TimeSpan Time { get; }
    public bool IsDeleted { get; set; } = false;
}

public class CarTiming
{
    public int CarNumber { get; }
    // can this be abstraccted?
    public readonly IList<IEvent> _events = new List<IEvent>();

    // Projection (Current State)
    private readonly CurrentState _currentState = new();

    public CarTiming(int carNumber)
    {
        CarNumber = carNumber;
    }

    public void LapCompleted(int lapNumber, string gap, TimeSpan time)
    {
        // domain rules / validation
        AddEvent(new LapCompleted(lapNumber, this.CarNumber, gap, time));
    }

    public void DeleteLapTime(int lapNumber)
    {
        AddEvent(new LapDeleted(lapNumber));
    }

    internal void AddEvent(IEvent evnt)
    {
        switch (evnt)
        {
            case LapCompleted lapCompleted:
                Apply(lapCompleted);
                break;
            case LapDeleted lapDeleted:
                Apply(lapDeleted);
                break;
            default: throw new NotSupportedException($"Unsupported Event: {nameof(evnt.GetType)}");
        }

        _events.Add(evnt);
    }

    private void Apply(LapCompleted lapCompleted)
    {
        _currentState.LapsCompleted.Add(lapCompleted.LapNumber, new LapTiming(lapCompleted.Time));
    }

    private void Apply(LapDeleted lapDeleted)
    {
        _currentState.LapsCompleted[lapDeleted.LapNumber].IsDeleted = true;
    }

    internal IList<IEvent> GetEvents()
    {
        return _events;
    }

    internal int GetLapsCompleted() => _currentState.LapsCompleted.Count;

    internal TimeSpan? GetFastestLap()
    {
        return _currentState.LapsCompleted.Values.Where(x => !x.IsDeleted)
                                                 .OrderBy(x => x.Time)
                                                 .FirstOrDefault()?.Time;
    }

}
