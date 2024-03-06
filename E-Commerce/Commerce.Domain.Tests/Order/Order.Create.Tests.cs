using Xunit;
using FluentAssertions;
using Commerce.SharedKernel.Domain;

namespace Commerce.Domain.Tests;

public sealed class OrderCreateTests
{
    [Fact]
    public void Given_Create_When_NoProductsProvided_Then_ShouldFail()
    {
        // Act
        var result = Order.Create(Guid.NewGuid(), "client name", "shipping address", new List<OrderedProduct>(), 100);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Order.Create.ProductsEmpty);
    }

    [Fact]
    public void Given_Create_When_ShippingAddressIsNullOrEmpty_Then_ShouldFail()
    {
        // Act
        var result = Order.Create(Guid.NewGuid(), "client name", string.Empty, new List<OrderedProduct> { OrderedProductsFactory.Any() }, 100);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Order.Create.ShippingAddressNullOrEmpty);
    }

    [Fact]
    public void Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var now = TimeProviderContext.AdvanceTimeToNow();

        var products = new List<OrderedProduct> { OrderedProductsFactory.Any() };
        var clientId = Guid.NewGuid();
        var clientName = "client name";
        var shippingAddress = "shipping address";
        var total = 100;

        // Act
        var result = Order.Create(clientId, clientName, shippingAddress, products, total);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.ClientId.Should().Be(clientId);
        result.Value.ClientName.Should().Be(clientName);
        result.Value.ShippingAddress.Should().Be(shippingAddress);
        result.Value.Products.Should().BeEquivalentTo(products);
        result.Value.Total.Value.Should().Be(total);
        result.Value.PlacedAt.Should().Be(now);
    }

    [Fact]
    public void Given_VerboseCreate_Then_ShouldPassThroughCreate_And_ModifyLeftOverProperties()
    {
        // Arrange
        var now = TimeProviderContext.AdvanceTimeToNow();

        var id = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var clientName = "client name";
        var shippingAddress = "shipping address";
        var products = new List<OrderedProduct> { OrderedProductsFactory.Any() };
        var total = 100;

        // Act
        var result = Order.Create(id, clientId, clientName, shippingAddress, products, total);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.ClientId.Should().Be(clientId);
        result.Value.ClientName.Should().Be(clientName);
        result.Value.ShippingAddress.Should().Be(shippingAddress);
        result.Value.Products.Should().BeEquivalentTo(products);
        result.Value.Total.Value.Should().Be(total);
        result.Value.PlacedAt.Should().Be(now);
    }
}