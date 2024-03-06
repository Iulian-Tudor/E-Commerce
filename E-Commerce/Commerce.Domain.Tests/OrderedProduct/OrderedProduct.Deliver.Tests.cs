using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class OrderedProductDeliverTests
{
    private readonly OrderedProduct sut = OrderedProductsFactory.Any().Processing();

    [Fact]
    public void Given_Deliver_When_StatusIsNotProcessing_Then_ShouldFail()
    {
        // Arrange
        sut.Fulfilled();

        // Act
        var result = sut.Deliver();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.OrderedProduct.Deliver.NotProcessing);
    }

    [Fact]
    public void Given_Deliver_When_StatusIsPending_Then_ShouldSucceed()
    {
        // Act
        var result = sut.Deliver();

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Status.Should().Be(OrderStatus.InDelivery);
    }
}