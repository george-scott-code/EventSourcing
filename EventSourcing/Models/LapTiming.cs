
namespace EventSourcing;

public class LapTiming
{
    public LapTiming(TimeSpan time)
    {
        Time = time;
    }

    public TimeSpan Time { get; }
    public bool IsDeleted { get; set; } = false;
}
