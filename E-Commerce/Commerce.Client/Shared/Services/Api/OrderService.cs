namespace Commerce.Client;

public interface IOrderService
{
    Task<ApiResult> PlaceOrder(PlaceOrderModel model);
}

public sealed class OrderService(IHttpClient http) : IOrderService
{
    public Task<ApiResult> PlaceOrder(PlaceOrderModel model) => http.Post(Routes.Orders.Place, model);
}