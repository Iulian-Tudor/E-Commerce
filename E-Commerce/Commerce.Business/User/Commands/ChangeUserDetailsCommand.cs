using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record ChangeUserDetailsCommand(Guid UserId, string FirstName, string LastName) : IRequest<Result>;