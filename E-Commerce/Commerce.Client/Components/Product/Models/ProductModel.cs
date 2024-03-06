namespace Commerce.Client;

public sealed class ProductModel
{
	public Guid Id { get; set; } = Guid.Empty;

	public Guid CategoryId { get; set; } = Guid.Empty;
    
    public Guid VendorId { get; set; } = Guid.Empty;

    public string VendorName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsVisible { get; set; }

    public MediaAssetModel MediaAsset { get; set; } = new();

    public byte[] Image { get; set; } = Array.Empty<byte>();

    public string ImageUrl => $"data:image/png;base64,{Convert.ToBase64String(Image)}";
}
