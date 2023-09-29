
using EventSourcing.Events;

namespace EventSourcing;

// demo storage, to be implemented in lib / db of choice

public interface ITimingRepository
{
    public IList<CarTiming> Get();
    public CarTiming Get(int carNumber);
    public void Save(CarTiming carTiming);
}

public class TimingRepository : ITimingRepository
{
    private readonly Dictionary<int, IList<IEvent>> _inMemoryStreams = new();

    public IList<CarTiming> Get()
    {
        var timings = new List<CarTiming>();

        foreach(var timing in _inMemoryStreams)
        {
            var carTiming = new CarTiming(timing.Key);

            foreach (var evnt in timing.Value)
            {
                carTiming.AddEvent(evnt);
            }

            timings.Add(carTiming);
        }
        return timings;
    }

    public CarTiming Get(int carNumber)
    {
        var carTiming = new CarTiming(carNumber);

        if (_inMemoryStreams.ContainsKey(carNumber))
        {
            foreach (var evnt in _inMemoryStreams[carNumber])
            {
                carTiming.AddEvent(evnt);
            }
        }
        return carTiming;
    }

    public void Save(CarTiming carTiming)
    {
        _inMemoryStreams[carTiming.CarNumber] = carTiming.GetEvents();
    }
}
