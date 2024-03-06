using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using Errors = Commerce.Domain.DomainErrors.Order;
using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Domain;

public sealed class Order : AggregateRoot
{
    private Order(Guid clientId, string clientName, string shippingAddress, IReadOnlyCollection<OrderedProduct> products, FixedPrecisionPrice total)
    {
        ClientId = clientId;
        ClientName = clientName;
        ShippingAddress = shippingAddress;
        Products = products;
        Total = total;
        PlacedAt = TimeProvider.Instance().UtcNow;
    }

    public static Result<Order> Create(Guid id, Guid clientId, string clientName, string shippingAddress, IReadOnlyCollection<OrderedProduct> products, FixedPrecisionPrice total)
        => Create(clientId, clientName, shippingAddress, products, total).Tap(o => o.Id = id);

    public static Result<Order> Create(Guid clientId, string clientName, string shippingAddress, IReadOnlyCollection<OrderedProduct> products, FixedPrecisionPrice total)
    {
        var productsResult = Result.SuccessIf(products.Count > 0, Errors.Create.ProductsEmpty);
        var shippingAddressResult = shippingAddress.EnsureNotNullOrEmpty(Errors.Create.ShippingAddressNullOrEmpty);

        return Result
            .FirstFailureOrSuccess(productsResult, shippingAddressResult)
            .Map(() => new Order(clientId, clientName, shippingAddress, products, total));
    }

    public Guid ClientId { get; private set; }

    public string ClientName { get; private set; }

    public string ShippingAddress { get; private set; }

    public IReadOnlyCollection<OrderedProduct> Products { get; private set; }

    public FixedPrecisionPrice Total { get; private set; }

    public DateTime PlacedAt { get; private set; }
}
