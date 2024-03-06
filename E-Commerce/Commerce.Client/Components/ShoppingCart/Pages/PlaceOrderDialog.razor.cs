using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace Commerce.Client.Components.ShoppingCart.Pages;

public sealed partial class PlaceOrderDialog
{
    [CascadingParameter] 
    MudDialogInstance MudDialog { get; set; }

    [SupplyParameterFromForm]
    public PlaceOrderModel? Model { get; set; } = new();

    private async Task Submit()
    {
        var identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsFailure)
        {
            NavigationManager.NavigateTo("/categories");
            return;
        }

        Model.ClientId = identifiedUserResult.Value.Id;
        
        var cart = await ShoppingCartService.Get();
        Model.Products = cart.Products.SelectMany(p => Enumerable.Repeat(p.Key, p.Value)).ToList();

        var response = await OrderService.PlaceOrder(Model);
        
        MudDialog.Close(DialogResult.Ok(true));

        if (response.IsSuccess)
        {
            await ShoppingCartService.Clear(false);
            NavigationManager.NavigateTo("categories");
        }
    }

    private void Cancel()
    {
        Model = new();
        MudDialog.Cancel();
    }
}