using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record CreateUserGateCommand(string EmailAddress) : IRequest<Result<CreateUserGateCommandResponse>>;

public sealed record CreateUserGateCommandResponse(Guid UserId);