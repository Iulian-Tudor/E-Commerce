using System.ComponentModel.DataAnnotations;

namespace Commerce.Client;

public sealed class PlaceOrderModel
{
    public Guid ClientId { get; set; }

    [Required]
    public string ShippingAddress { get; set; } = string.Empty;

    public IReadOnlyCollection<Guid> Products { get; set; } = new List<Guid>();
}