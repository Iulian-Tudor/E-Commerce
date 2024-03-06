using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record DeliverOrderedProductCommand(Guid OrderId, Guid OrderedProductId) : IRequest<Result>;