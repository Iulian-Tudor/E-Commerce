using Commerce.Domain;
using Commerce.Business;
using CSharpFunctionalExtensions;

namespace Commerce.Infrastructure;

internal sealed class FavoriteProductSnapshotRepository(EfDbContext dbContext) : IFavoriteProductSnapshotRepository
{
    public async Task<Maybe<FavoriteProductSnapshot>> Load(Guid id)
    {
        Maybe<FavoriteProductSnapshotReadModel> favoriteProductSnapshot = await dbContext.FavoriteProductSnapshots.FindAsync(id);

        return favoriteProductSnapshot.HasValue
            ? Maybe<FavoriteProductSnapshot>.From(favoriteProductSnapshot.Value.ToAggregate())
            : Maybe<FavoriteProductSnapshot>.None;
    }

    public IQueryable<FavoriteProductSnapshotReadModel> Query() => dbContext.FavoriteProductSnapshots.AsQueryable();

    public async Task Store(FavoriteProductSnapshot aggregate)
    {
        var existingFavoriteProductSnapshot = await dbContext.FavoriteProductSnapshots.FindAsync(aggregate.Id);

        if (existingFavoriteProductSnapshot is not null)
        {
            dbContext.Entry(existingFavoriteProductSnapshot).CurrentValues.SetValues(new FavoriteProductSnapshotReadModel().FromAggregate(aggregate));
        }
        else
        {
            var readModel = new FavoriteProductSnapshotReadModel().FromAggregate(aggregate);
            await dbContext.FavoriteProductSnapshots.AddAsync(readModel);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var favoriteProductSnapshot = await dbContext.FavoriteProductSnapshots.FindAsync(id);

        if (favoriteProductSnapshot is not null)
        {
            dbContext.FavoriteProductSnapshots.Remove(favoriteProductSnapshot);
            await dbContext.SaveChangesAsync();
        }
    }
}