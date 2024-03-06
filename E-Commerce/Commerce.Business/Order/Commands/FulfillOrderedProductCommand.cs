using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record FulfillOrderedProductCommand(Guid OrderId, Guid OrderedProductId) : IRequest<Result>;