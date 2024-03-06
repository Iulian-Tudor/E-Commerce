namespace Commerce.Client.Components.Order.Pages;

public sealed partial class MyOrders
{
    public IReadOnlyCollection<OrderedProductModel> OrderedProducts { get; set; } = new List<OrderedProductModel>();

    protected override async Task OnInitializedAsync()
    {
        var identifiedUserResult = await AuthService.GetIdentifiedUser();
        if (identifiedUserResult.IsFailure)
        {
            NavigationManager.NavigateTo("/categories");
            return;
        }

        OrderedProducts = await OrderedProductService.GetAllForClient(identifiedUserResult.Value.Id);
    }
}