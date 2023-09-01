namespace EventSourcing;

// demo storage, to be implemented in lib / db of choice
public class TimingRepository
{
    private readonly Dictionary<int, IList<IEvent>> _inMemoryStreams = new();

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
