using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record ProcessOrderedProductCommand(Guid OrderId, Guid OrderedProductId) : IRequest<Result>;