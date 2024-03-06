using System.ComponentModel.DataAnnotations;

namespace Commerce.Client;

public sealed class ChangeUserDetailsModel
{

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name is too long."), MinLength(2, ErrorMessage = "First name is too short.")] 
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name is too long."), MinLength(2, ErrorMessage = "Last name is too short.")]
    public string LastName { get; set; }
}