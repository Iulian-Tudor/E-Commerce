using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class OrderedProductFulfillTests
{
    private readonly OrderedProduct sut = OrderedProductsFactory.Any().InDelivery();

    [Fact]
    public void Given_Fulfill_When_StatusIsNotInDelivery_Then_ShouldFail()
    {
        // Arrange
        sut.Fulfilled();

        // Act
        var result = sut.Fulfill();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.OrderedProduct.Fulfill.NotInDelivery);
    }

    [Fact]
    public void Given_Fulfill_When_StatusIsPending_Then_ShouldSucceed()
    {
        // Act
        var result = sut.Fulfill();

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Status.Should().Be(OrderStatus.Fulfilled);
    }
}