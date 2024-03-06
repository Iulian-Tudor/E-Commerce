using MediatR;
using Commerce.Domain;

namespace Commerce.Business.QueryHandlers;

internal sealed class GetOrdersQueryHandler(IOrderedProductRepository repository) : IRequestHandler<GetOrdersQuery, IReadOnlyCollection<OrderedProduct>>
{
    public async Task<IReadOnlyCollection<OrderedProduct>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = repository.Query();

        if (request.Id.HasValue)
        {
            orders = orders.Where(p => p.Id == request.Id);
        }

        if (request.VendorId.HasValue)
        {
            orders = orders.Where(p => p.VendorId == request.VendorId);
        }

        if (request.ClientId.HasValue)
        {
            orders = orders.Where(p => p.ClientId == request.ClientId);
        }

        if (request.Ids.Any())
        {
            orders = orders.Where(p => request.Ids.Contains(p.Id));
        }

        return orders.ToList();
    }
}