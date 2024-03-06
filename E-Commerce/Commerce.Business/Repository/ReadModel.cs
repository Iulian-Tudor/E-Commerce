using Commerce.SharedKernel.Domain;

namespace Commerce.Business;

public abstract class ReadModel<TAggregate> where TAggregate : AggregateRoot
{
    public Guid Id { get; set; }

    public abstract TAggregate ToAggregate();
}