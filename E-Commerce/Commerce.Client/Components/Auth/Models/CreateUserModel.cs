using System.ComponentModel.DataAnnotations;

namespace Commerce.Client;

public sealed class CreateUserModel
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name is too long."), MinLength(2, ErrorMessage = "First name is too short.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name is too long."), MinLength(2, ErrorMessage = "Last name is too short.")]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    [Required(ErrorMessage = "Email address is required")]
    [StringLength(50, ErrorMessage = "Email address is too long."), MinLength(5, ErrorMessage = "Email address is too short.")]
    public string EmailAddress { get; set; } = string.Empty;
}