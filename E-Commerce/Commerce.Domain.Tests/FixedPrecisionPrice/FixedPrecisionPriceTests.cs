using FluentAssertions;
using Xunit;

namespace Commerce.Domain.Tests;

public sealed class FixedPrecisionPriceTests
{
    [Fact]
    public void Given_Create_When_ValueHasMoreThanTwoDecimalPlaces_Then_ShouldRoundDown()
    {
        // Act
        var result = new FixedPrecisionPrice(100.123456m);

        // Assert
        result.Value.Should().Be(100.12m);
    }

    [Fact]
    public void Given_DecimalCast_Then_ShouldReturnValue()
    {
        // Arrange
        var sut = new FixedPrecisionPrice(100.123456m);

        // Act
        decimal result = sut;

        // Assert
        result.Should().Be(100.12m);
    }

    [Fact]
    public void Given_FixedPrecisionPriceCast_Then_ShouldReturnFixedPrecisionPriceObject()
    {
        // Arrange
        var value = 100.123456m;

        // Act
        var result = (FixedPrecisionPrice)value;

        // Assert
        result.Value.Should().Be(100.12m);
    }
}