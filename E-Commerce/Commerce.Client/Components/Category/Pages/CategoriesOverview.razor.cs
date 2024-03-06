using CSharpFunctionalExtensions;

namespace Commerce.Client.Components.Category.Pages;

public sealed partial class CategoriesOverview
{
    private Result<IdentifiedUser> identifiedUserResult;
    private IReadOnlyCollection<CategoryModel> categories = new List<CategoryModel>();

    protected override async Task OnInitializedAsync()
    {
        identifiedUserResult = await AuthService.GetIdentifiedUser();
        categories = await CategoryService.GetAll();
    }

    private void NavigateToAddProduct(Guid categoryId)
    {
        NavigationManager.NavigateTo($"/categories/{categoryId}/products/create");
    }

    private void NavigateToProductsOverview(Guid categoryId)
    {
        NavigationManager.NavigateTo($"/categories/{categoryId}/products");
    }
}