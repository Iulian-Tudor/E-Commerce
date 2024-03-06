using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class OrderedProductCreateTests
{
    [Fact]
    public void Given_Create_Then_ShouldCreate()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var vendorName = "vendor name";
        var name = "name";
        var description = "description";
        var price = 100m;
        var count = 10;
        var isVisible = true;

        // Act
        var result = new OrderedProduct(productId, categoryId, vendorId, clientId, vendorName, name, description, price, count, isVisible);

        // Assert
        result.ProductId.Should().Be(productId);
        result.CategoryId.Should().Be(categoryId);
        result.VendorId.Should().Be(vendorId);
        result.ClientId.Should().Be(clientId);
        result.VendorName.Should().Be(vendorName);
        result.Name.Should().Be(name);
        result.Description.Should().Be(description);
        result.Price.Should().Be(price);
        result.Count.Should().Be(count);
        result.IsVisible.Should().Be(isVisible);
        result.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void Given_SetOrderId_Then_ShouldSetOrderId()
    {
        // Arrange
        var sut = new OrderedProduct(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "vendor name", "name", "description", 100m, 10, true);
        var orderId = Guid.NewGuid();

        // Act
        sut.SetOrderId(orderId);

        // Assert
        sut.OrderId.Should().Be(orderId);
    }

    [Fact]
    public void Given_SetShippingAddress_Then_ShouldSetShippingAddress()
    {
        // Arrange
        var sut = new OrderedProduct(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "vendor name", "name", "description", 100m, 10, true);
        var shippingAddress = "shipping address";

        // Act
        sut.SetShippingAddress(shippingAddress);

        // Assert
        sut.ShippingAddress.Should().Be(shippingAddress);
    }
}