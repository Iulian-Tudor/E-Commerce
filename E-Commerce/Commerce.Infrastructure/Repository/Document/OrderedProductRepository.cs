using Commerce.Domain;
using Commerce.Business;
using CSharpFunctionalExtensions;

namespace Commerce.Infrastructure;

internal sealed class OrderedProductRepository(EfDbContext dbContext) : IOrderedProductRepository
{
    public async Task<Maybe<OrderedProduct>> Load(Guid id)
    {
        Maybe<OrderedProduct> product = await dbContext.OrderedProducts.FindAsync(id);
        return product;
    }

    public IQueryable<OrderedProduct> Query() => dbContext.OrderedProducts.AsQueryable();

    public async Task Store(OrderedProduct snapshot)
    {
        var existingProducts = await dbContext.OrderedProducts.FindAsync(snapshot.Id);
        
        if (existingProducts is not null)
        {
            dbContext.Entry(existingProducts).CurrentValues.SetValues(snapshot);
        }
        else
        {
            await dbContext.OrderedProducts.AddAsync(snapshot);
        }

        await dbContext.SaveChangesAsync();
    }
}