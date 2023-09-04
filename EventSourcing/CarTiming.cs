namespace EventSourcing;

public class CurrentState
{
    public int LapsCompleted { get; internal set; }
    public TimeSpan? FastestLap { get; internal set; } = null;
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

    internal void AddEvent(IEvent evnt)
    {
        switch (evnt)
        {
            case LapCompleted lapCompleted:
                Apply(lapCompleted);
                break;
            default: throw new NotSupportedException($"Unsupported Event: {nameof(evnt.GetType)}");
        }

        _events.Add(evnt);
    }

    private void Apply(LapCompleted lapCompleted)
    {
        _currentState.LapsCompleted += 1;
        if (_currentState.FastestLap is null || lapCompleted.Time < _currentState.FastestLap)
            _currentState.FastestLap = lapCompleted.Time;
    }

    internal IList<IEvent> GetEvents()
    {
        return _events;
    }

    internal int GetLapsCompleted() => _currentState.LapsCompleted;

    internal TimeSpan? GetFastestLap() => _currentState.FastestLap;

}
