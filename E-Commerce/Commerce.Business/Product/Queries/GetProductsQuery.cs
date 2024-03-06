using MediatR;

namespace Commerce.Business;

public sealed record GetProductsQuery : IRequest<IReadOnlyCollection<ProductAggregationModel>>
{
    public Guid? Id { get; init; }

    public IReadOnlyCollection<Guid> Ids { get; init; } = new List<Guid>();

    public Guid? VendorId { get; init; }

    public Guid? CategoryId { get; init; }

    public bool? IsVisible { get; init; }
}