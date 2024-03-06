using MediatR;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.FavoriteProductSnapshot.Delete;

namespace Commerce.Business;

internal sealed class DeleteFavoriteProductCommandHandler(IFavoriteProductSnapshotRepository favoriteProductSnapshotRepository, IIdentifiedUser identifiedUser) : IRequestHandler<DeleteFavoriteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteFavoriteProductCommand request, CancellationToken cancellationToken)
    {
        var favoriteProductSnapshotResult = await favoriteProductSnapshotRepository
            .Load(request.FavoriteProductSnapshotId)
            .ToResult(Errors.FavoriteProductSnapshotNotFound);

        var ownershipResult = favoriteProductSnapshotResult
            .Ensure(fps => fps.UserId == identifiedUser.Id, Errors.FavoriteProductSnapshotNotOwnedByCaller);

        return await Result
            .FirstFailureOrSuccess(favoriteProductSnapshotResult, ownershipResult)
            .Tap(() => favoriteProductSnapshotRepository.Delete(request.FavoriteProductSnapshotId));
    }
}