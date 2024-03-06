using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.Product.Pages;

public sealed partial class CreateProduct
{
    private Result<IdentifiedUser> identifiedUserResult;

    [Parameter]
    public string CategoryId { get; set; }

    [SupplyParameterFromForm]
    public CreateProductModel? Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsFailure)
        {
            NavigationManager.NavigateTo("/categories");
        }
    }

    private async Task Submit()
    {
        Model.CategoryId = Guid.Parse(CategoryId);
        Model.VendorId = identifiedUserResult.Value.Id;

        var response = await ProductService.Create(Model);
        if (response.IsSuccess)
        {
            NavigationManager.NavigateTo($"/categories/{CategoryId}/products");
        }
    }

    private void Cancel()
    {
        NavigationManager.NavigateTo($"/categories/{CategoryId}/products");
    }
}