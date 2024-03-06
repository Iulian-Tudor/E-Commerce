using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class OrderedProductProcessTests
{
    private readonly OrderedProduct sut = OrderedProductsFactory.Any().Confirmed();

    [Fact]
    public void Given_Process_When_StatusIsNotConfirmed_Then_ShouldFail()
    {
        // Arrange
        sut.InDelivery();

        // Act
        var result = sut.Process();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.OrderedProduct.Process.NotConfirmed);
    }

    [Fact]
    public void Given_Process_When_StatusIsPending_Then_ShouldSucceed()
    {
        // Act
        var result = sut.Process();

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Status.Should().Be(OrderStatus.Processing);
    }
}