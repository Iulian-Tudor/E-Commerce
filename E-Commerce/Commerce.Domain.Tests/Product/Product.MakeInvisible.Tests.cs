using FluentAssertions;
using Xunit;

namespace Commerce.Domain.Tests;

public sealed class ProductMakeInvisibleTests
{
	private readonly Product sut = ProductsFactory.Any();

	[Fact]
	public void Given_MakeInvisible_When_ProductAlreadyInvisible_Then_ShouldFail()
	{
		// Arrange
		sut.MakeInvisible();

		// Act
		var result = sut.MakeInvisible();

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Product.MakeInvisible.ProductAlreadyInvisible);
	}

	[Fact]
	public void Given_MakeInvisible_When_NotViolatingConstraints_Then_ShouldSuccess()
	{
		// Arrange
		sut.MakeVisible();

		// Act
		var result = sut.MakeInvisible();

		// Assert
		result.IsSuccess.Should().BeTrue();
		sut.IsVisible.Should().BeFalse();
	}
}