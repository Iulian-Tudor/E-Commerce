using MediatR;
using Commerce.Domain;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.User.Create;

namespace Commerce.Business;

internal sealed class CreateUserCommandHandler(IUserRepository repository, IEmailService emailService) : IRequestHandler<CreateUserCommand, Result>
{
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUsersCount = repository
            .Query()
            .Count(u => u.EmailAddress == request.EmailAddress);

        return await Result
            .SuccessIf(existingUsersCount <= 0, Errors.EmailAddressAlreadyInUse)
            .Bind(() => User.Create(request.FirstName, request.LastName, request.EmailAddress))
            .Tap(u =>
            {
                emailService.SendWelcome(u);
            })
            .Tap(repository.Store);
    }
}