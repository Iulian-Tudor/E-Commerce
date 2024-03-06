using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.User.Delete;

namespace Commerce.Business;

internal sealed class DeleteUserCommandHandler(IUserRepository userRepository, IOrderRepository orderRepository, IProductRepository productRepository) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userResult = await userRepository
            .Load(request.UserId)
            .ToResult(Errors.UserNotFound);
        
        //TODO: delete related stuff after they are implemented

        return await userResult.Tap(u => userRepository.Delete(u.Id));
    }
}