using Commerce.Domain;

namespace Commerce.Business;

public class ProductReadModel : ReadModel<Product>
{
    public Guid CategoryId { get; set; } = Guid.Empty;

    public Guid VendorId { get; set; } = Guid.Empty;

    public string VendorName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsVisible { get; set; }

    public MediaAsset? MediaAsset { get; set; }

    public override Product ToAggregate() => Product.Create(Id, VendorId, VendorName, Name, Description, Price, CategoryId, IsVisible, MediaAsset).Value;

    public ProductReadModel FromAggregate(Product aggregate)
    {
        Id = aggregate.Id;
        CategoryId = aggregate.CategoryId;
        VendorId = aggregate.VendorId;
        VendorName = aggregate.VendorName;
        Name = aggregate.Name;
        Description = aggregate.Description;
        Price = aggregate.Price;
        IsVisible = aggregate.IsVisible;
        MediaAsset = aggregate.MediaAsset.HasValue ? aggregate.MediaAsset.Value : null;

        return this;
    }
}
