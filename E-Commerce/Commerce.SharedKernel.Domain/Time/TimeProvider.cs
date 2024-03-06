namespace Commerce.SharedKernel.Domain;

public static class TimeProvider
{
    private static readonly Lazy<ITimeProvider> TimeProviderInstance = new(() => new DefaultTimeProvider());

    public static ITimeProvider Instance()
    {
        return TimeProviderInstance.Value;
    }
}