using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record RefreshTokenCommand(string ExpiredToken, string RefreshToken) : IRequest<Result<RefreshTokenCommandResponse>>;

public sealed record RefreshTokenCommandResponse(string AuthToken, Token RefreshToken);