
namespace Commerce.Domain;

public static class CategoriesFactory
{
    public static Category Any() => WithId(Guid.NewGuid());

    public static Category WithId(Guid id) => Category.Create(id, "category", "a description!!!!").Value;
}