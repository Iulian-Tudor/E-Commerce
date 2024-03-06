using Commerce.Domain;
using Commerce.Business;
using CSharpFunctionalExtensions;

namespace Commerce.Infrastructure;

internal sealed class UserRepository(EfDbContext dbContext) : IUserRepository
{
    public async Task<Maybe<User>> Load(Guid id)
    {
        Maybe<UserReadModel> user = await dbContext.Users.FindAsync(id);

        return user.HasValue
            ? Maybe<User>.From(user.Value.ToAggregate())
            : Maybe<User>.None;
    }

    public IQueryable<UserReadModel> Query() => dbContext.Users.AsQueryable();

    public async Task Store(User aggregate)
    {
        var existingUser = await dbContext.Users.FindAsync(aggregate.Id);

        if (existingUser is not null)
        {
            dbContext.Entry(existingUser).CurrentValues.SetValues(new UserReadModel().FromAggregate(aggregate));
        }
        else
        {
            var readModel = new UserReadModel().FromAggregate(aggregate);
            await dbContext.Users.AddAsync(readModel);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if (user is not null)
        {
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
        }
    }
}