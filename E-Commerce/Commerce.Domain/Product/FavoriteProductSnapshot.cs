using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;
using Errors = Commerce.Domain.DomainErrors.FavoriteProductSnapshot.Create;

namespace Commerce.Domain;

public sealed class FavoriteProductSnapshot : AggregateRoot
{
    public FavoriteProductSnapshot(Guid userId, Guid productId, FixedPrecisionPrice initialPrice, DateTime createdAt)
    {
        UserId = userId;
        ProductId = productId;
        InitialPrice = initialPrice;
        CreatedAt = createdAt;
    }

    public static Result<FavoriteProductSnapshot> Create(Guid id, Guid userId, Guid productId, FixedPrecisionPrice initialPrice, DateTime createdAt)
        => Result.Success(new FavoriteProductSnapshot(userId, productId, initialPrice, createdAt)).Tap(f => f.Id = id);

    public static Result<FavoriteProductSnapshot> Create(Guid userId, Product product)
    {
        var userIdResult = userId.EnsureNotEmpty(Errors.UserIdEmpty);
        var productResult = product.EnsureNotNull(Errors.ProductNull);

        return Result
            .FirstFailureOrSuccess(userIdResult, productResult)
            .Map(() => new FavoriteProductSnapshot(userId, product.Id, product.Price, TimeProvider.Instance().UtcNow));
    }

    public Guid UserId { get; private set; }

    public Guid ProductId { get; private set; }

    public FixedPrecisionPrice InitialPrice { get; private set; }

    public DateTime CreatedAt { get; private set; }
}