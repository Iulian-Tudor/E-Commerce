using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.UserGate.Exchange;

namespace Commerce.Business;

internal class ExchangeUserGateCommandHandler(IUserRepository userRepository, IUserGateRepository userGateRepository, ITokenService tokenService ) : IRequestHandler<ExchangeUserGateCommand, Result<ExchangeUserGateCommandResponse>>
{
    public async Task<Result<ExchangeUserGateCommandResponse>> Handle(ExchangeUserGateCommand request, CancellationToken cancellationToken)
    {
        var userResult = await userRepository
            .Load(request.UserId)
            .ToResult(Errors.UserNotFound);

        var userGateResult = await Maybe<UserGateReadModel>
            .From(userGateRepository
                .Query()
                .Where(g => g.UserId == request.UserId)
                .OrderByDescending(g => g.CreatedAt)
                .FirstOrDefault()
            )
            .Bind(ug => userGateRepository.Load(ug.Id))
            .ToResult(Errors.UserGateNotFound);

        var tokenResult = userResult
            .Bind(tokenService.GenerateAuthToken);

        return await Result
            .FirstFailureOrSuccess(userResult, userGateResult, tokenResult)
            .Bind(() => userGateResult.Value.Exchange(request.GateSecret))
            .Tap(() => userGateRepository.Store(userGateResult.Value))
            .Map(() => tokenService.GenerateRefreshToken(userResult.Value))
            .Map(rt => new ExchangeUserGateCommandResponse(tokenResult.Value, rt));
    }
}
