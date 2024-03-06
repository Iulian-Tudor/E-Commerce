using Commerce.Domain;

namespace Commerce.Business;

public sealed class FavoriteProductSnapshotReadModel : ReadModel<FavoriteProductSnapshot>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid ProductId { get; set; } = Guid.Empty;

    public decimal InitialPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public override FavoriteProductSnapshot ToAggregate() => FavoriteProductSnapshot.Create(Id, UserId, ProductId, InitialPrice, CreatedAt).Value;

    public FavoriteProductSnapshotReadModel FromAggregate(FavoriteProductSnapshot aggregate)
    {
        Id = aggregate.Id;
        UserId = aggregate.UserId;
        ProductId = aggregate.ProductId;
        InitialPrice = aggregate.InitialPrice;
        CreatedAt = aggregate.CreatedAt;

        return this;
    }
}