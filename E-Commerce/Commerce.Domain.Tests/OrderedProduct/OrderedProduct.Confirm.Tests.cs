using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class OrderedProductConfirmTests
{
    private readonly OrderedProduct sut = OrderedProductsFactory.Any();

    [Fact]
    public void Given_Confirm_When_StatusIsNotPending_Then_ShouldFail()
    {
        // Arrange
        sut.InDelivery();

        // Act
        var result = sut.Confirm();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.OrderedProduct.Confirm.NotPending);
    }

    [Fact]
    public void Given_Confirm_When_StatusIsPending_Then_ShouldSucceed()
    {
        // Act
        var result = sut.Confirm();

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Status.Should().Be(OrderStatus.Confirmed);
    }
}