using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record ExchangeUserGateCommand(Guid UserId, string GateSecret) : IRequest<Result<ExchangeUserGateCommandResponse>>;

public sealed record ExchangeUserGateCommandResponse(string AuthToken, Token RefreshToken);