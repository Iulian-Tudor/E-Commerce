using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.Category.Pages;

public sealed partial class CreateCategory
{
    [SupplyParameterFromForm]
    public CreateCategoryModel? Model { get; set; } = new();

    private async Task Submit()
    {
        var response = await CategoryService.Create(Model);
        if (response.IsSuccess)
        {
            NavigationManager.NavigateTo("categories");
        }
    }

    private void Cancel()
    {
        NavigationManager.NavigateTo("categories");
    }
}