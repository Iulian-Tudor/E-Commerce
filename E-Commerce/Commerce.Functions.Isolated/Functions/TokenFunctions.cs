using MediatR;
using Commerce.Business;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Commerce.Functions.Isolated;

public class TokenFunctions(IMediator mediator)
{
    [Function(nameof(RefreshToken))]
    public async Task<HttpResponseData> RefreshToken([HttpTrigger(AuthorizationLevel.Anonymous, HttpVerbs.Post, Route = "v1/token/refresh")] HttpRequestData req)
    {
        return await req
            .DeserializeBodyPayload<RefreshTokenCommand>()
            .Bind(c => mediator.Send(c))
            .ToResponseData(req, (resp, res) => resp.WriteAsJsonAsync(res));
    }
}