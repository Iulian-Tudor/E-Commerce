using Commerce.Domain;
using Commerce.Business;
using CSharpFunctionalExtensions;

namespace Commerce.Infrastructure;

internal sealed class ProductRepository(EfDbContext dbContext) : IProductRepository
{
    public async Task<Maybe<Product>> Load(Guid id)
    {
        Maybe<ProductReadModel> product = await dbContext.Products.FindAsync(id);

        return product.HasValue
            ? Maybe<Product>.From(product.Value.ToAggregate())
            : Maybe<Product>.None;
    }

    public IQueryable<ProductReadModel> Query() => dbContext.Products.AsQueryable();

    public async Task Store(Product aggregate)
    {
        var existingProduct = await dbContext.Products.FindAsync(aggregate.Id);

        if(existingProduct is not null)
        {
            dbContext.Entry(existingProduct).CurrentValues.SetValues(new ProductReadModel().FromAggregate(aggregate));
            existingProduct.MediaAsset = aggregate.MediaAsset.HasValue ? aggregate.MediaAsset.Value : null;
        }
        else
        {
            var readModel = new ProductReadModel().FromAggregate(aggregate);
            await dbContext.Products.AddAsync(readModel);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var product = await dbContext.Products.FindAsync(id);

        if (product is not null)
        {
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }
 }
