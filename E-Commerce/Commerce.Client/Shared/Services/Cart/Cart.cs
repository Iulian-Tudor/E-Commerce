namespace Commerce.Client;

public sealed class Cart(Dictionary<Guid, int> products)
{
    public Dictionary<Guid, int> Products { get; set; } = products;
}