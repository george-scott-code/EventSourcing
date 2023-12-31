
using EventSourcing.Events;
namespace EventSourcing;

public class CurrentState
{
    public Dictionary<int, LapTiming> LapsCompleted { get; set; } = new Dictionary<int, LapTiming>();
}

public class CarTiming
{
    public int CarNumber { get; }
    // can this be abstracted?
    public readonly IList<RaceEvent> _events = new List<RaceEvent>();

    // Projection (Current State)
    private readonly CurrentState _currentState = new();

    public CarTiming(int carNumber)
    {
        CarNumber = carNumber;
    }

    public void LapCompleted(int lapNumber, string gap, TimeSpan lapTime)
    {
        // domain rules / validation
        AddEvent(new LapCompletedEvent(lapNumber, CarNumber, gap, lapTime));
    }

    public void DeleteLapTime(int lapNumber)
    {
        AddEvent(new LapDeletedEvent(lapNumber, CarNumber));
    }

    internal void AddEvent(RaceEvent evnt)
    {
        switch (evnt)
        {
            case LapCompletedEvent lapCompleted:
                Apply(lapCompleted);
                break;
            case LapDeletedEvent lapDeleted:
                Apply(lapDeleted);
                break;
            default: throw new NotSupportedException($"Unsupported Event: {nameof(evnt.GetType)}");
        }

        _events.Add(evnt);
    }

    private void Apply(LapCompletedEvent lapCompleted)
    {
        _currentState.LapsCompleted.Add(lapCompleted.LapNumber, new LapTiming(lapCompleted.Time));
    }

    private void Apply(LapDeletedEvent lapDeleted)
    {
        _currentState.LapsCompleted[lapDeleted.LapNumber].IsDeleted = true;
    }

    internal IList<RaceEvent> GetEvents()
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
