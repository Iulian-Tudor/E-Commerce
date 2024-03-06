using MediatR;
using Commerce.Domain;
using CSharpFunctionalExtensions;

using Errors = Commerce.Business.BusinessErrors.FavoriteProductSnapshot.Create;

namespace Commerce.Business;

internal sealed class AddProductToFavoritesCommandHandler(IProductRepository productRepository, IFavoriteProductSnapshotRepository favoriteProductSnapshotRepository, IIdentifiedUser identifiedUser) : IRequestHandler<AddProductToFavoritesCommand, Result>
{
    public async Task<Result> Handle(AddProductToFavoritesCommand request, CancellationToken cancellationToken)
    {
        var productResult = await productRepository
            .Load(request.ProductId)
            .ToResult(Errors.ProductNotFound);

        var favoriteProductSnapshot = Maybe.From(favoriteProductSnapshotRepository
            .Query()
            .FirstOrDefault(fps => fps.ProductId == request.ProductId && fps.UserId == identifiedUser.Id));
        var favoriteProductSnapshotResult = Result.FailureIf(favoriteProductSnapshot.HasValue, Errors.ProductAlreadyInFavorites);

        return await Result
            .FirstFailureOrSuccess(productResult, favoriteProductSnapshotResult)
            .Bind(() => FavoriteProductSnapshot.Create(identifiedUser.Id, productResult.Value))
            .Tap(favoriteProductSnapshotRepository.Store);
    }
}