using MediatR;
using System.Net;
using Commerce.Business;
using Commerce.Infrastructure;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Commerce.Functions.Isolated;

public sealed class UserFunctions(IMediator mediator)
{
    [Function(nameof(OptionsHandler))]
    public static async Task<HttpResponseData> OptionsHandler([HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = "{*any}")] HttpRequestData req, FunctionContext executionContext)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);

        var requestOrigin = req.Headers.TryGetValues("Origin", out var origin);
        if (requestOrigin)
        {
            response.Headers.Add("Access-Control-Allow-Origin", origin.First());
        }
        else
        {
            response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5167");
        }

        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PATCH, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

        return response;
    }

    [Function(nameof(CreateUser))]
    public async Task<HttpResponseData> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/users")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<CreateUserCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(GetUser))]
    public async Task<HttpResponseData> GetUser([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/users/{id}")] HttpRequestData req, Guid id)
    {
        var query = new GetUsersQuery { Id = id };
        var users = await mediator.Send(query);

        return await users.TryFirst().ToResponseData(req);
    }

    [Function(nameof(GetUsers))]
    public async Task<HttpResponseData> GetUsers([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Get, Route = "v1/users")] HttpRequestData req)
    {
        var query = new GetUsersQuery();
        var users = await mediator.Send(query);

        return await req.CreateJsonResponse(users);
    }

    [FunctionRequiresAuth]
    [Function(nameof(ChangeUserDetails))]
    public async Task<HttpResponseData> ChangeUserDetails([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Put, Route = "v1/users/{id}/details")] HttpRequestData req, Guid id)
    {
        return await req
            .DeserializeBodyPayload<ChangeUserDetailsCommand>()
            .Map(c => c with { UserId = id })
            .Bind(c => mediator.Send(c))
            .ToResponseData(req);
    }

    [FunctionRequiresAuth]
    [Function(nameof(DeleteUser))]
    public async Task<HttpResponseData> DeleteUser([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Delete, Route = "v1/users/{id}")] HttpRequestData req, Guid id)
    {
        return await mediator.Send(new DeleteUserCommand(id)).ToResponseData(req);
    }

    [Function(nameof(CreateUserGate))]
    public async Task<HttpResponseData> CreateUserGate([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/user-gates")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<CreateUserGateCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req, (resp, res) => resp.WriteAsJsonAsync(res));
    }

    [Function(nameof(PassUserGate))]
    public async Task<HttpResponseData> PassUserGate([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/user-gates/passage")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<PassUserGateCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req, (resp, res) => resp.WriteAsJsonAsync(res));
    }

    [Function(nameof(ExchangeUserGate))]
    public async Task<HttpResponseData> ExchangeUserGate([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Patch, Route = "v1/user-gates/exchange")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<ExchangeUserGateCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req, (resp, res) => resp.WriteAsJsonAsync(res));
    }
}