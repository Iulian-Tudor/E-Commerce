using CSharpFunctionalExtensions;
using Commerce.SharedKernel.Domain;

using Errors = Commerce.Domain.DomainErrors.OrderedProduct;

namespace Commerce.Domain;

public sealed class OrderedProduct : Document
{
    public OrderedProduct(Guid productId, Guid categoryId, Guid vendorId, Guid clientId, string vendorName, string name, string description, decimal price, int count, bool isVisible)
    {
        ProductId = productId;
        CategoryId = categoryId;
        VendorId = vendorId;
        ClientId = clientId;
        VendorName = vendorName;
        Name = name;
        Description = description;
        Price = price;
        Count = count;
        IsVisible = isVisible;
        Status = OrderStatus.Pending;
    }

    public Guid ProductId { get; init; }

    public Guid CategoryId { get; init; }

    public Guid VendorId { get; init; }

    public Guid ClientId { get; init; }

    public Guid OrderId { get; private set; }

    public string VendorName { get; init; }

    public string Name { get; init; }

    public string Description { get; init; }

    public decimal Price { get; init; }

    public int Count { get; init; }

    public FixedPrecisionPrice TotalPrice => Price * Count;
    
    public bool IsVisible { get; init; }

    public OrderStatus Status { get; private set; }

    public string ShippingAddress { get; private set; } = string.Empty;

    public void SetOrderId(Guid orderId)
    {
        OrderId = orderId;
    }

    public void SetShippingAddress(string shippingAddress)
    {
        ShippingAddress = shippingAddress;
    }

    public Result Confirm()
    {
        return Result
            .SuccessIf(Status is OrderStatus.Pending, Errors.Confirm.NotPending)
            .Tap(() => Status = OrderStatus.Confirmed);
    }

    public Result Process()
    {
        return Result
            .SuccessIf(Status is OrderStatus.Confirmed, Errors.Process.NotConfirmed)
            .Tap(() => Status = OrderStatus.Processing);
    }

    public Result Deliver()
    {
        return Result
            .SuccessIf(Status is OrderStatus.Processing, Errors.Deliver.NotProcessing)
            .Tap(() => Status = OrderStatus.InDelivery);
    }

    public Result Fulfill()
    {
        return Result
            .SuccessIf(Status is OrderStatus.InDelivery, Errors.Fulfill.NotInDelivery)
            .Tap(() => Status = OrderStatus.Fulfilled);
    }
}