using MediatR;
using Commerce.Domain;

namespace Commerce.Business;

public sealed record GetOrdersQuery : IRequest<IReadOnlyCollection<OrderedProduct>>
{
    public Guid? Id { get; init; }

    public IReadOnlyCollection<Guid> Ids { get; init; } = new List<Guid>();

    public Guid? VendorId { get; init; }

    public Guid? ClientId { get; init; }
}