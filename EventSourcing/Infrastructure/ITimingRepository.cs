namespace EventSourcing.Infrastructure;

// demo storage, to be implemented in lib / db of choice

public interface ITimingRepository
{
    public IList<CarTiming> Get();
    public IList<CarTiming> GetToLap(int lap);
    public CarTiming Get(int carNumber);
    public void Save(CarTiming carTiming);
}
