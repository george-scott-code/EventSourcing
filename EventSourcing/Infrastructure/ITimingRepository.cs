namespace EventSourcing.Infrastructure;

// demo storage, to be implemented in lib / db of choice

public interface ITimingRepository
{
    public IList<CarTiming> Get();
    public CarTiming Get(int carNumber);
    public void Save(CarTiming carTiming);
}
