using MediatR;
using Commerce.Business;
using Commerce.Infrastructure;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Commerce.Functions.Isolated;

public sealed class OrderFunctions(IMediator mediator)
{
    [FunctionRequiresAuth]
    [Function(nameof(PlaceOrder))]
    public async Task<HttpResponseData> PlaceOrder([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/orders")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<PlaceOrderCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(ConfirmOrderedProduct))]
    public async Task<HttpResponseData> ConfirmOrderedProduct([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/orders/{orderId}/products/{orderedProductId}/confirmation")] HttpRequestData req, Guid orderId, Guid orderedProductId)
    {
        return await mediator
            .Send(new ConfirmOrderedProductCommand(orderId, orderedProductId))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(ProcessOrderedProduct))]
    public async Task<HttpResponseData> ProcessOrderedProduct([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/orders/{orderId}/products/{orderedProductId}/process")] HttpRequestData req, Guid orderId, Guid orderedProductId)
    {
        return await mediator
            .Send(new ProcessOrderedProductCommand(orderId, orderedProductId))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(DeliverOrderedProduct))]
    public async Task<HttpResponseData> DeliverOrderedProduct([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/orders/{orderId}/products/{orderedProductId}/delivery")] HttpRequestData req, Guid orderId, Guid orderedProductId)
    {
        return await mediator
            .Send(new DeliverOrderedProductCommand(orderId, orderedProductId))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(FulfillOrderedProduct))]
    public async Task<HttpResponseData> FulfillOrderedProduct([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/orders/{orderId}/products/{orderedProductId}/fulfillment")] HttpRequestData req, Guid orderId, Guid orderedProductId)
    {
        return await mediator
            .Send(new FulfillOrderedProductCommand(orderId, orderedProductId))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(GetOrder))]
    public async Task<HttpResponseData> GetOrder([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/orders/{id}")] HttpRequestData req, Guid id)
    {
        var query = new GetOrdersQuery { Id = id };
        var orders = await mediator.Send(query);

        return await orders.TryFirst().ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(GetOrders))]
    public async Task<HttpResponseData> GetOrders([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/orders")] HttpRequestData req)
    {
        var filter = req.ParseQuery<OrdersQueryFilter>();
        var query = new GetOrdersQuery
        {
            Id = filter.Id,
            VendorId = filter.VendorId,
            Ids = filter.Ids,
            ClientId = filter.ClientId
        };

        var orders = await mediator.Send(query);

        return await req.CreateJsonResponse(orders);
    }

    public sealed class OrdersQueryFilter
    {
        public Guid? Id { get; init; }

        public IReadOnlyCollection<Guid> Ids { get; init; } = new List<Guid>();

        public Guid? VendorId { get; init; }

        public Guid? ClientId { get; init; }
    }
}
