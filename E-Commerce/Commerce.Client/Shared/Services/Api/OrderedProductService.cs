namespace Commerce.Client;

public interface IOrderedProductService
{
    Task<OrderedProductModel?> Get(Guid id);

    Task<IReadOnlyCollection<OrderedProductModel>> GetAll();

    Task<IReadOnlyCollection<OrderedProductModel>> GetMany(List<Guid> ids);

    Task<IReadOnlyCollection<OrderedProductModel>> GetAllByVendor(Guid vendorId);

    Task<IReadOnlyCollection<OrderedProductModel>> GetAllForClient(Guid clientId);

    Task<ApiResult> Confirm(OrderedProductModel model);

    Task<ApiResult> Process(OrderedProductModel model);

    Task<ApiResult> Deliver(OrderedProductModel model);

    Task<ApiResult> Fulfill(OrderedProductModel model);
}

public sealed class OrderedProductService(IHttpClient http) : IOrderedProductService
{
    public async Task<OrderedProductModel?> Get(Guid id) => await http.Get<OrderedProductModel>(Routes.OrderedProducts.Get.WithRouteParams(new { Id = id }));

    public async Task<IReadOnlyCollection<OrderedProductModel>> GetAll() 
        => await http.Get<IReadOnlyCollection<OrderedProductModel>>(Routes.OrderedProducts.GetAll) ?? new List<OrderedProductModel>();

    public async Task<IReadOnlyCollection<OrderedProductModel>> GetMany(List<Guid> ids) 
        => await http.Get<IReadOnlyCollection<OrderedProductModel>>(Routes.OrderedProducts.GetAll.WithQueryParams(new { ids })) ?? new List<OrderedProductModel>();

    public async Task<IReadOnlyCollection<OrderedProductModel>> GetAllByVendor(Guid vendorId) 
        => await http.Get<IReadOnlyCollection<OrderedProductModel>>(Routes.OrderedProducts.GetAll.WithQueryParams(new { vendorId })) ?? new List<OrderedProductModel>();

    public async Task<IReadOnlyCollection<OrderedProductModel>> GetAllForClient(Guid clientId)
        => await http.Get<IReadOnlyCollection<OrderedProductModel>>(Routes.OrderedProducts.GetAll.WithQueryParams(new { clientId })) ?? new List<OrderedProductModel>();

    public async Task<ApiResult> Confirm(OrderedProductModel model) 
        => await http.Patch(Routes.OrderedProducts.Confirm.WithRouteParams(new { orderId = model.OrderId, orderedProductId = model.Id }), model);

    public async Task<ApiResult> Process(OrderedProductModel model) 
        => await http.Patch(Routes.OrderedProducts.Process.WithRouteParams(new { orderId = model.OrderId, orderedProductId = model.Id }), model);

    public async Task<ApiResult> Deliver(OrderedProductModel model) 
        => await http.Patch(Routes.OrderedProducts.Deliver.WithRouteParams(new { orderId = model.OrderId, orderedProductId = model.Id }), model);

    public async Task<ApiResult> Fulfill(OrderedProductModel model) 
        => await http.Patch(Routes.OrderedProducts.Fulfill.WithRouteParams(new { orderId = model.OrderId, orderedProductId = model.Id }), model);
}