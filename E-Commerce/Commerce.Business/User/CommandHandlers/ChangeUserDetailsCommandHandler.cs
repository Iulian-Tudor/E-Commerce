using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.User.ChangeDetails;

namespace Commerce.Business;

internal sealed class ChangeUserDetailsCommandHandler(IUserRepository repository) : IRequestHandler<ChangeUserDetailsCommand, Result>
{
    public async Task<Result> Handle(ChangeUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userResult = await repository
            .Load(request.UserId)
            .ToResult(Errors.UserNotFound);

        return await userResult
            .Check(u => u.ChangeDetails(request.FirstName, request.LastName))
            .Tap(repository.Store);
    }
}