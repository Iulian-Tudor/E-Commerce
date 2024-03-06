namespace Commerce.Client;

public sealed class AddProductToFavoritesModel
{
    public Guid ProductId { get; set; } = Guid.Empty;
}