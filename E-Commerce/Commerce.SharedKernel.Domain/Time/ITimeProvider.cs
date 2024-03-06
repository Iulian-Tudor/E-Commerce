namespace Commerce.SharedKernel.Domain;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}