using MediatR;
using Commerce.Business;
using Commerce.Infrastructure;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Commerce.Functions.Isolated;

public sealed class CategoryFunctions(IMediator mediator, ICategoryRepository repository)
{
    [FunctionRequiresAuth]
    [Function(nameof(CreateCategory))]
    public async Task<HttpResponseData> CreateCategory([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/categories")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<CreateCategoryCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req);
    }

    [Function(nameof(GetCategories))]
    public async Task<HttpResponseData> GetCategories([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/categories")] HttpRequestData req)
    {
        var categories = repository.Query().ToList();

        return await req.CreateJsonResponse(categories);
    }

    [Function(nameof(GetCategory))]

    public async Task<HttpResponseData> GetCategory([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/categories/{id}")] HttpRequestData req, Guid id)
    {
        var category = repository.Query().Where(c => c.Id == id).TryFirst();

        return await category.ToResponseData(req);
    }
}