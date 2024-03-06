using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.Product.Pages;

public sealed partial class ProductOverview
{
    private ProductModel? product;
    private IReadOnlyCollection<FavoriteProductModel> favoriteProducts = new List<FavoriteProductModel>();
    private Result<IdentifiedUser> identifiedUserResult;

    [Parameter]
    public string CategoryId { get; set; }

    [Parameter]
    public string ProductId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        product = await ProductService.Get(Guid.Parse(ProductId));

        identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsSuccess)
        {
            favoriteProducts = await ProductService.GetFavorites();
        }
    }

    private void EditProduct()
    {
        NavigationManager.NavigateTo($"/categories/{CategoryId}/products/{ProductId}/edit");
    }


    private void RemoveFromFavorites(ProductModel product)
    {
        ProductService.RemoveFromFavorites(product.Id);
    }
}