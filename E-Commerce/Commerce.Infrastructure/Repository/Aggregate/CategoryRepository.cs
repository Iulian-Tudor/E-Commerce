using Commerce.Domain;
using Commerce.Business;
using CSharpFunctionalExtensions;

namespace Commerce.Infrastructure;

internal sealed class CategoryRepository(EfDbContext dbContext) : ICategoryRepository
{
    public async Task<Maybe<Category>> Load(Guid id)
    {
        Maybe<CategoryReadModel> category = await dbContext.Categories.FindAsync(id);

        return category.HasValue
            ? Maybe<Category>.From(category.Value.ToAggregate())
            : Maybe<Category>.None;
    }

    public IQueryable<CategoryReadModel> Query() => dbContext.Categories.AsQueryable();

    public async Task Store(Category aggregate)
    {
        var existingCategory = await dbContext.Categories.FindAsync(aggregate.Id);

        if (existingCategory is not null)
        {
            dbContext.Entry(existingCategory).CurrentValues.SetValues(aggregate);
        }
        else
        {
            var readModel = new CategoryReadModel().FromAggregate(aggregate);
            await dbContext.Categories.AddAsync(readModel);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var category = await dbContext.Categories.FindAsync(id);

        if (category is not null)
        {
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
        }
    }
}