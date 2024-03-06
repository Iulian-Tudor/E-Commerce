using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.Product.Pages;

public sealed partial class ProductsOverview
{
    private IReadOnlyCollection<ProductModel> products = new List<ProductModel>();
    private IReadOnlyCollection<FavoriteProductModel> favoriteProducts = new List<FavoriteProductModel>();
    private Result<IdentifiedUser> identifiedUserResult;
    private CategoryModel? category = new();

    [Parameter]
    public string CategoryId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        category = await CategoryService.Get(Guid.Parse(CategoryId));
        products = await ProductService.GetAllByCategory(Guid.Parse(CategoryId));

        identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsSuccess)
        {
            favoriteProducts = await ProductService.GetFavorites();
        }
    }

    private void NavigateToProduct(Guid productId)
    {
        NavigationManager.NavigateTo($"/categories/{CategoryId}/products/{productId}");
    }

    private void NavigateToAddProduct()
    {
        NavigationManager.NavigateTo($"/categories/{CategoryId}/products/create");
    }

    private void RemoveFromFavorites(ProductModel product)
    {
        ProductService.RemoveFromFavorites(product.Id);
        favoriteProducts = favoriteProducts.Where(fp => fp.ProductId != product.Id).ToList();
    }
}