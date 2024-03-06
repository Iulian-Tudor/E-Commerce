using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

namespace Commerce.Business;

public interface IDocumentRepository<TDocument> where TDocument : Document
{
    Task<Maybe<TDocument>> Load(Guid id);

    IQueryable<TDocument> Query();

    Task Store(TDocument snapshot);
}