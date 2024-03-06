namespace Commerce.SharedKernel.Domain;

public abstract class AggregateRoot
{
    protected AggregateRoot()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; protected set; }
}