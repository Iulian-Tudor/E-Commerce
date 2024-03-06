using FluentAssertions;
using Xunit;

namespace Commerce.Domain.Tests;

public sealed class ProductMakeVisibleTests
{
	private readonly Product sut = ProductsFactory.Any();

	[Fact]
	public void Given_MakeVisible_When_ProductAlreadyVisible_Then_ShouldFail()
	{
		// Arrange
		sut.MakeVisible();

		// Act
		var result = sut.MakeVisible();

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Product.MakeVisible.ProductAlreadyVisible);
	}

	[Fact]
	public void Given_MakeVisible_When_NotViolatingConstraints_Then_ShouldSuccess()
	{
		// Arrange
		sut.MakeInvisible();

		// Act
		var result = sut.MakeVisible();

		// Assert
		result.IsSuccess.Should().BeTrue();
		sut.IsVisible.Should().BeTrue();
	}
}