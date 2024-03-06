using System.ComponentModel.DataAnnotations;

namespace Commerce.Client;

public sealed class PassUserGateModel
{
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Pass code is required.")]
    [StringLength(6, ErrorMessage = "Pass code must be exactly 6 characters long.")]
    public string PassCode { get; set; }
}

public sealed class PassUserGateResponse
{
    public string GateSecret { get; set; }
}