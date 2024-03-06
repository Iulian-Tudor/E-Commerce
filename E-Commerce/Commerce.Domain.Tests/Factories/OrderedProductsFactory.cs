namespace Commerce.Domain;

public static class OrderedProductsFactory
{
    public static OrderedProduct Any() => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "mike", "name", "a proper description", 10, 5, true);

    public static OrderedProduct WithIds(Guid productId, Guid categoryId, Guid vendorId) => new(productId, categoryId, vendorId, Guid.NewGuid(), "mike", "name", "a proper description", 10, 5, true);

    public static OrderedProduct Confirmed(this OrderedProduct product)
    {
        product.Confirm();

        return product;
    }

    public static OrderedProduct Processing(this OrderedProduct product)
    {
        product.Confirmed().Process();

        return product;
    }

    public static OrderedProduct InDelivery(this OrderedProduct product)
    {
        product.Confirmed().Processing().Deliver();

        return product;
    }

    public static OrderedProduct Fulfilled(this OrderedProduct product)
    {
        product.Confirmed().Processing().InDelivery().Fulfill();

        return product;
    }
}