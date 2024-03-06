using CSharpFunctionalExtensions;

namespace Commerce.Client.Components.Product.Pages;

public sealed partial class AllProductsOverview
{
    private IReadOnlyCollection<ProductModel> products = new List<ProductModel>();
    private IReadOnlyCollection<FavoriteProductModel> favoriteProducts = new List<FavoriteProductModel>();
    private Result<IdentifiedUser> identifiedUserResult;

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetAll();

        identifiedUserResult = await AuthService.GetIdentifiedUser();

        if (identifiedUserResult.IsSuccess)
        {
            favoriteProducts = await ProductService.GetFavorites();
        }
    }

    private void NavigateToProduct(ProductModel product)
    {
        NavigationManager.NavigateTo($"/categories/{product.CategoryId}/products/{product.Id}");
    }

    private void NavigateToCategory(ProductModel product)
    {
        NavigationManager.NavigateTo($"/categories/{product.CategoryId}/products");
    }

    private void RemoveFromFavorites(ProductModel product)
    {
        ProductService.RemoveFromFavorites(product.Id);
        favoriteProducts = favoriteProducts.Where(fp => fp.ProductId != product.Id).ToList();
    }
}