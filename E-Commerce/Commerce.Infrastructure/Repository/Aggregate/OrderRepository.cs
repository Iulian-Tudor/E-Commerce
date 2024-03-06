using Commerce.Domain;
using Commerce.Business;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Infrastructure;

internal sealed class OrderRepository(EfDbContext dbContext) : IOrderRepository
{
    public async Task<Maybe<Order>> Load(Guid id)
    {
        Maybe<OrderReadModel> order = await dbContext.Orders
            .Include(o => o.Products)
            .SingleOrDefaultAsync(o => o.Id == id);

        return order.HasValue
            ? Maybe<Order>.From(order.Value.ToAggregate())
            : Maybe<Order>.None;
    }

    public IQueryable<OrderReadModel> Query() => dbContext.Orders.Include(o => o.Products).AsQueryable();

    public async Task Store(Order aggregate)
    {
        dbContext.Orders.Add(new OrderReadModel().FromAggregate(aggregate));
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var order = await dbContext.Orders.FindAsync(id);

        if (order is not null)
        {
            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync();
        }
    }
}