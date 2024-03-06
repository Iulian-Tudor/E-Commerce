namespace Commerce.SharedKernel.Domain;

internal class DefaultTimeProvider : ITimeProvider
{
    public DateTime UtcNow => TimeProviderContext.Current?.Time ?? DateTime.UtcNow;
}