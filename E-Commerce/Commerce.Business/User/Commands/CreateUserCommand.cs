using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

public sealed record CreateUserCommand(string FirstName, string LastName, string EmailAddress) : IRequest<Result>;