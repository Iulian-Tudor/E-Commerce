using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;