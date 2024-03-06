namespace Commerce.Client;

public sealed class FavoriteProductModel
{
    public Guid Id { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

    public Guid ProductId { get; set; } = Guid.Empty;

    public decimal InitialPrice { get; set; }

    public DateTime CreatedAt { get; set; }
}