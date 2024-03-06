using MediatR;
using Commerce.Business;
using HttpMultipartParser;
using Commerce.Infrastructure;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Commerce.Functions.Isolated;

public sealed class ProductFunctions(IMediator mediator, IFavoriteProductSnapshotRepository favoriteProductSnapshotRepository, IIdentifiedUserAccessor identifiedUserAccessor)
{
    [FunctionRequiresAuth]
    [Function(nameof(CreateProduct))]
    public async Task<HttpResponseData> CreateProduct([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/products")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<CreateProductCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req);
    }

    [Function(nameof(GetProduct))]
    public async Task<HttpResponseData> GetProduct([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/products/{id}")] HttpRequestData req, Guid id)
    {
        var query = new GetProductsQuery { Id = id };
        var products = await mediator.Send(query);

        return await products.TryFirst().ToResponseData(req);
    }

    [Function(nameof(GetProducts))]
    public async Task<HttpResponseData> GetProducts([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/products")] HttpRequestData req)
    {
        var filter = req.ParseQuery<ProductsQueryFilter>();
        var query = new GetProductsQuery
        {
            Id = filter.Id,
            VendorId = filter.VendorId,
            IsVisible = filter.IsVisible,
            CategoryId = filter.CategoryId,
            Ids = filter.Ids
        };

        var products = await mediator.Send(query);

        return await req.CreateJsonResponse(products);
    }

    [Function(nameof(ChangeProductImage))]
    public async Task<HttpResponseData> ChangeProductImage([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/products/{id}/images")] HttpRequestData req, Guid id)
    {
        if (req.Body == Stream.Null)
        {
            return await Result
                .Failure("No files attached to request!")
                .ToResponseData(req);
        }

        var parsedRequest = await MultipartFormDataParser.ParseAsync(req.Body);
        if (!parsedRequest.Files.Any())
        {
            return await Result
                .Failure("No files attached to request!")
                .ToResponseData(req);
        }

        if (parsedRequest.Files.Count > 1)
        {
            return await Result
                .Failure("Exactly 1 file is expected to be attached to request!")
                .ToResponseData(req);
        }

        var file = parsedRequest.Files.First();
        var command = new ChangeProductImageCommand(id, file.Data, file.FileName, file.ContentType);

        return await mediator
            .Send(command)
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(ChangeProductCategories))]
    public async Task<HttpResponseData> ChangeProductCategories([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/products/{id}/categories")] HttpRequestData req, Guid id)
    {
        return await req
            .DeserializeBodyPayload<ChangeProductCategoryCommand>()
            .Map(p => p with { ProductId = id })
            .Bind(c => mediator.Send(c))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(ChangeProductDetails))]
    public async Task<HttpResponseData> ChangeProductDetails([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/products/{id}/details")] HttpRequestData req, Guid id)
    {
        return await req
            .DeserializeBodyPayload<ChangeProductDetailsCommand>()
            .Map(p => p with { ProductId = id })
            .Bind(p => mediator.Send(p))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(AddProductToFavorites))]
    public async Task<HttpResponseData> AddProductToFavorites([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/favorite-products")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<AddProductToFavoritesCommand>()
            .Bind(p => mediator.Send(p))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(RemoveProductFromFavorites))]
    public async Task<HttpResponseData> RemoveProductFromFavorites([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Delete, Route = "v1/favorite-products/{id}")] HttpRequestData req, Guid id)
    {
        return await mediator.Send(new DeleteFavoriteProductCommand(id)).ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(GetFavoriteProducts))]
    public async Task<HttpResponseData> GetFavoriteProducts([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/favorite-products")] HttpRequestData req)
    {
        var identifiedUser = identifiedUserAccessor.User;

        var products = identifiedUser.HasValue
            ? favoriteProductSnapshotRepository.Query()
                .Where(fps => fps.UserId == identifiedUser.Value.Id)
                .ToList()
            : [];

        return await req.CreateJsonResponse(products);
    }
  
    [FunctionRequiresAuth]
    [Function(nameof(DeleteProduct))]
    public async Task<HttpResponseData> DeleteProduct([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Delete, Route = "v1/products/{id}")] HttpRequestData req, Guid id)
    {
        return await mediator.Send(new DeleteProductCommand(id)).ToResponseData(req);
    }
  
    [FunctionRequiresAuth]
    [Function(nameof(MakeProductVisible))]
    public async Task<HttpResponseData> MakeProductVisible([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/products/{id}/visibility")] HttpRequestData req, Guid id)
    {
        return await mediator.Send(new MakeProductVisibleCommand(id)).ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(MakeProductInvisible))]
    public async Task<HttpResponseData> MakeProductInvisible([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Delete, Route = "v1/products/{id}/visibility")] HttpRequestData req, Guid id)
    {
        return await mediator.Send(new MakeProductInvisibleCommand(id)).ToResponseData(req);
    }

    public sealed class ProductsQueryFilter
    {
        public Guid? Id { get; private set; }

        public IReadOnlyCollection<Guid> Ids { get; private set; } = new List<Guid>();

        public Guid? VendorId { get; private set; }

        public Guid? CategoryId { get; private set; }

        public bool? IsVisible { get; private set; }
    }
}