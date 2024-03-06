using System.ComponentModel.DataAnnotations;

namespace Commerce.Client;

public sealed class CreateCategoryModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(25, ErrorMessage = "Category name is too long!"), MinLength(3, ErrorMessage = "Category name is too short!")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required!")]
    [StringLength(100, ErrorMessage = "Description is too long!"), MinLength(10, ErrorMessage = "Description is too short!")]
    public string Description { get; set; } = string.Empty;
}

