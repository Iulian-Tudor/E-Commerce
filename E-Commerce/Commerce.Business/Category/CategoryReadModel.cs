using Commerce.Domain;

namespace Commerce.Business;

public sealed class CategoryReadModel : ReadModel<Category>
{ 
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty; 

    public override Category ToAggregate() => Category.Create(Id, Name, Description).Value;

    public CategoryReadModel FromAggregate(Category aggregate)
    {
        Id = aggregate.Id;
        Name = aggregate.Name;
        Description = aggregate.Description;

        return this;
    }
}