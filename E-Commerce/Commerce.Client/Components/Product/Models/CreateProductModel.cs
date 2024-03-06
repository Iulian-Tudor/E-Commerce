using System.ComponentModel.DataAnnotations;

namespace Commerce.Client;

public sealed class CreateProductModel
{
    public Guid CategoryId { get; set; } = Guid.Empty;

    public Guid VendorId { get; set; } = Guid.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name is too long."), MinLength(3, ErrorMessage = "Name is too short.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(400, ErrorMessage = "Description is too long."), MinLength(10, ErrorMessage = "Description is too short.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }
}
