using MediatR;
using CSharpFunctionalExtensions;

namespace Commerce.Business;

internal sealed class RefreshTokenCommandHandler(ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenCommandResponse>>
{
    public Task<Result<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        tokenService
            .Refresh(request.ExpiredToken, request.RefreshToken)
            .Map(t => new RefreshTokenCommandResponse(t.AuthToken, t.RefreshToken));

}