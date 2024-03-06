using System.ComponentModel.DataAnnotations;

namespace Commerce.Client;

public sealed class CreateUserGateModel
{
    [Required(ErrorMessage = "Email address is required.")]
    [StringLength(50, ErrorMessage = "Email address is too long."), MinLength(5, ErrorMessage = "Email address is too short.")]
    public string EmailAddress { get; set; } = string.Empty;
}

public sealed class CreateUserGateResponse
{
    public Guid UserId { get; set; }
}