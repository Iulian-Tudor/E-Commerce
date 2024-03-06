namespace Commerce.SharedKernel.Domain;

public class TimeProviderContext : IDisposable
{
    private static readonly ThreadLocal<Stack<TimeProviderContext>> ThreadScopeStack = new(() => new Stack<TimeProviderContext>());

    private TimeProviderContext(DateTime currentTime)
    {
        Time = currentTime;
    }

    public static TimeProviderContext Current => ThreadScopeStack.Value.Count == 0 ? null : ThreadScopeStack.Value.Peek();

    public static DateTime AdvanceTimeTo(DateTime time)
    {
        ThreadScopeStack.Value.Push(new TimeProviderContext(time));
        return time;
    }

    public static DateTime AdvanceTimeToNow() => AdvanceTimeTo(DateTime.Now);

    public DateTime Time { get; }

    public void Dispose()
    {
        ThreadScopeStack.Value.Pop();
    }
}