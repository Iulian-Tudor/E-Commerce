namespace Commerce.Client.Components.Order.Pages;

public sealed partial class ReceivedOrdersOverview
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

        OrderedProducts = await OrderedProductService.GetAllByVendor(identifiedUserResult.Value.Id);
    }

    private async Task Confirm(OrderedProductModel model)
    {
        var result = await OrderedProductService.Confirm(model);
        if (result.IsSuccess)
        {
            OrderedProducts.First(op => op.Id == model.Id).Status = OrderStatus.Confirmed;
        }
    }

    private async Task Process(OrderedProductModel model)
    {
        var result = await OrderedProductService.Process(model);
        if (result.IsSuccess)
        {
            OrderedProducts.First(op => op.Id == model.Id).Status = OrderStatus.Processing;
        }
    }

    private async Task Deliver(OrderedProductModel model)
    {
        var result = await OrderedProductService.Deliver(model);
        if (result.IsSuccess)
        {
            OrderedProducts.First(op => op.Id == model.Id).Status = OrderStatus.InDelivery;
        }
    }

    private async Task Fulfill(OrderedProductModel model)
    {
        var result = await OrderedProductService.Fulfill(model);
        if (result.IsSuccess)
        {
            OrderedProducts.First(op => op.Id == model.Id).Status = OrderStatus.Fulfilled;
        }
    }
}