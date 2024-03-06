namespace Commerce.Domain;

public static class ProductsFactory
{
    public static Product Any() => WithId(Guid.NewGuid());

    public static Product WithId(Guid id) => Product.Create(id, "mike", "product", "a description!!!!", 10, Guid.NewGuid()).Value;

    public static Product Visible(this Product product)
    {
        product.MakeVisible();

        return product;
    }
}