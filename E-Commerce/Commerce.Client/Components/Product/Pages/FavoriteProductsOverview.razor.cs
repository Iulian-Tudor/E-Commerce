namespace Commerce.Client.Components.Product.Pages;

public sealed partial class FavoriteProductsOverview
{
    public IReadOnlyCollection<FavoriteProductModel> FavoriteProducts { get; set; } = new List<FavoriteProductModel>();

    public IReadOnlyCollection<ProductModel> Products { get; set; } = new List<ProductModel>();

    protected override async Task OnInitializedAsync()
    {
        var identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsFailure)
        {
            NavigationManager.NavigateTo("/categories");
            return;
        }

        FavoriteProducts = await ProductService.GetFavorites();
        Products = await ProductService.GetMany(FavoriteProducts.Select(fp => fp.ProductId).ToList());
    }

    private async Task AddToCart(Guid productId)
    {
        await ShoppingCartService.Add(productId);
    }

    private async Task RemoveFromFavorites(Guid productId)
    {
        var favoriteId = FavoriteProducts.First(fp => fp.ProductId == productId).Id;
        var result = await ProductService.RemoveFromFavorites(favoriteId);

        if (result.IsSuccess)
        {
            FavoriteProducts = FavoriteProducts.Where(fp => fp.Id != favoriteId).ToList();
            Products = Products.Where(p => p.Id != productId).ToList();
        }
    }
}