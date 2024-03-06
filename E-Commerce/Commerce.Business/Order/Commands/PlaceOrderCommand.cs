using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record PlaceOrderCommand(Guid ClientId, string ShippingAddress, IReadOnlyCollection<Guid> Products) : IRequest<Result>;