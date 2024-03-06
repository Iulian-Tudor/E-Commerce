using Commerce.Domain;
using Commerce.Business;
using CSharpFunctionalExtensions;

namespace Commerce.Infrastructure;

internal sealed class UserGateRepository(EfDbContext dbContext) : IUserGateRepository
{
    public async Task<Maybe<UserGate>> Load(Guid id)
    {
        Maybe<UserGateReadModel> userGate = await dbContext.UserGates.FindAsync(id);

        return userGate.HasValue
            ? Maybe<UserGate>.From(userGate.Value.ToAggregate())
            : Maybe<UserGate>.None;
    }

    public IQueryable<UserGateReadModel> Query() => dbContext.UserGates.AsQueryable();

    public async Task Store(UserGate aggregate)
    {
        var existingUserGate = await dbContext.UserGates.FindAsync(aggregate.Id);

        if (existingUserGate is not null)
        {
            dbContext.Entry(existingUserGate).CurrentValues.SetValues(new UserGateReadModel().FromAggregate(aggregate));
        }
        else
        {
            var readModel = new UserGateReadModel().FromAggregate(aggregate);
            await dbContext.UserGates.AddAsync(readModel);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var userGate = await dbContext.UserGates.FindAsync(id);

        if (userGate is not null)
        {
            dbContext.UserGates.Remove(userGate);
            await dbContext.SaveChangesAsync();
        }
    }
}