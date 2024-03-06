using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

namespace Commerce.Business;

public interface IRepository<TAggregate, TReadModel> where TAggregate : AggregateRoot where TReadModel : ReadModel<TAggregate>
{
    Task<Maybe<TAggregate>> Load(Guid id);

    IQueryable<TReadModel> Query();

    Task Store(TAggregate aggregate);

    Task Delete(Guid id);
}