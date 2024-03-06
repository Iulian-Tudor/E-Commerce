namespace Commerce.Domain;

public enum OrderStatus
{
    Pending = 100,
    Confirmed = 200,
    Processing = 300,
    InDelivery = 400,
    Fulfilled = 500
}