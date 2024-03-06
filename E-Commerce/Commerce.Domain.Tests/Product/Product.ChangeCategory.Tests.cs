using FluentAssertions;
using Xunit;

namespace Commerce.Domain.Tests;

public sealed class ProductChangeCategoryTests
{
    private readonly Product sut = ProductsFactory.Any();

    [Fact]
    public void Given_ChangeCategory_When_CategoryIdIsEmpty_Then_ShouldFail()
    {
        // Arrange
        var categoryId = Guid.Empty;
        // Act
        var result = sut.ChangeCategory(categoryId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeCategory.CategoryIdEmpty);
    }

    [Fact]
    public void Given_ChangeCategory_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        // Act
        var result = sut.ChangeCategory(categoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}

