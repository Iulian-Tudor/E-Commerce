namespace Commerce.Client;

public sealed class OrderedProductModel
{
    public Guid Id { get; init; } = Guid.Empty;

    public Guid ProductId { get; init; } = Guid.Empty;

    public Guid CategoryId { get; init; } = Guid.Empty;

    public Guid VendorId { get; init; } = Guid.Empty;

    public Guid ClientId { get; init; } = Guid.Empty;

    public Guid OrderId { get; init; } = Guid.Empty;

    public string ShippingAddress { get; init; } = "-";

    public string VendorName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int Count { get; init; }

    public OrderStatus Status { get; set; }
}

public enum OrderStatus
{
    Pending = 100,
    Confirmed = 200,
    Processing = 300,
    InDelivery = 400,
    Fulfilled = 500
}