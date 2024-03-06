using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record PassUserGateCommand(Guid UserId, string PassCode) : IRequest<Result<PassUserGateCommandResponse>>;

public sealed record PassUserGateCommandResponse(string GateSecret);