namespace Commerce.Domain;

public static class OrdersFactory
{
    public static Order WithDetails(Guid clientId, IReadOnlyCollection<OrderedProduct> products)
        => Order.Create(clientId, "client name", "shipping address", products, 100).Value;
}