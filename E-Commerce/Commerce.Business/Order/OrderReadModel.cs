using Commerce.Domain;

namespace Commerce.Business;

public sealed class OrderReadModel : ReadModel<Order>
{
    private List<OrderedProduct> products = new();

    public Guid ClientId { get; set; } = Guid.Empty;

    public string ClientName { get; private set; } = string.Empty;

    public string ShippingAddress { get; private set; } = string.Empty;

    public IReadOnlyCollection<OrderedProduct> Products => products;

    public decimal Total { get; set; } 

    public DateTime PlacedAt { get; set; }

    public override Order ToAggregate() => Order.Create(Id, ClientId, ClientName, ShippingAddress, Products, Total).Value;

    public OrderReadModel FromAggregate(Order aggregate)
    {
        Id = aggregate.Id;
        ClientId = aggregate.ClientId;
        ClientName = aggregate.ClientName;
        ShippingAddress = aggregate.ShippingAddress;
        products = aggregate.Products.ToList();
        Total = aggregate.Total;
        PlacedAt = aggregate.PlacedAt;

        return this;
    }
}