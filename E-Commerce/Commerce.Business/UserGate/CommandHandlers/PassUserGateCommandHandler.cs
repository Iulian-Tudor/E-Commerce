using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.UserGate.Pass;

namespace Commerce.Business;

internal sealed class PassUserGateCommandHandler(IUserRepository userRepository, IUserGateRepository userGateRepository) : IRequestHandler<PassUserGateCommand, Result<PassUserGateCommandResponse>>
{
    public async Task<Result<PassUserGateCommandResponse>> Handle(PassUserGateCommand request, CancellationToken cancellationToken)
    {
        var userResult = await userRepository
            .Load(request.UserId)
            .ToResult(Errors.UserNotFound);

        var userGateResult = await Maybe<UserGateReadModel>
            .From(userGateRepository
                .Query()
                .Where(ug => ug.UserId == request.UserId)
                .OrderByDescending(ug => ug.CreatedAt)
                .FirstOrDefault()
            )
            .Bind(ug => userGateRepository.Load(ug.Id))
            .ToResult(Errors.UserGateNotFound);

        return await Result
            .FirstFailureOrSuccess(userResult, userGateResult)
            .Bind(() => userGateResult.Value.Pass(request.PassCode))
            .Tap(() => userGateRepository.Store(userGateResult.Value))
            .Map(() => new PassUserGateCommandResponse(userGateResult.Value.Secret));
    }
}