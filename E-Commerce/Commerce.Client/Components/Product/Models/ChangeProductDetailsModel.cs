namespace Commerce.Client;

public sealed class ChangeProductDetailsModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }
}