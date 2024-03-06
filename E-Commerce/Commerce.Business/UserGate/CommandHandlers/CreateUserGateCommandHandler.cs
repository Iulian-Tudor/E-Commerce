using MediatR;
using Commerce.Domain;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.UserGate.Create;

namespace Commerce.Business;

internal class CreateUserGateCommandHandler(IUserRepository userRepository, IUserGateRepository userGateRepository, IEmailService emailService) : IRequestHandler<CreateUserGateCommand, Result<CreateUserGateCommandResponse>>
{
    public async Task<Result<CreateUserGateCommandResponse>> Handle(CreateUserGateCommand request, CancellationToken cancellationToken)
    {
        var userResult = Maybe<UserReadModel>
            .From(userRepository.Query().Where(u => u.EmailAddress == request.EmailAddress).FirstOrDefault())
            .ToResult(Errors.UserNotFound);

        return await userResult
            .Bind(u => UserGate.Create(u.Id))
            .Tap(ug => { emailService.SendUserGate(userResult.Value, ug.PassCode); })
            .Tap(userGateRepository.Store)
            .Map(ug => new CreateUserGateCommandResponse(ug.UserId));
    }
}