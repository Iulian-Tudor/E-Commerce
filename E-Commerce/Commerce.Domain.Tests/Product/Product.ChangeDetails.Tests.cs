using FluentAssertions;
using Xunit;

namespace Commerce.Domain.Tests;

public sealed class ProductChangeDetailsTests
{
    private readonly Product sut = ProductsFactory.Any();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_ChangeDetails_When_NameIsNullOrEmpty_Then_ShouldFail(string name)
    {
        // Act
        var result = sut.ChangeDetails(name, "longDescription", 1m);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeDetails.NameNullOrEmpty);
    }

    [Fact]
    public void Given_ChangeDetails_When_NameIsShorterThanMin_Then_ShouldFail()
    {
        // Act
        var result = sut.ChangeDetails(new string('a', DomainConstants.Product.NameMinLength - 1), "longDescription", (decimal)1.1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeDetails.NameShorterThanMinLength);
    }

    [Fact]
    public void Given_ChangeDetails_When_NameIsLongerThanMax_Then_ShouldFail()
    {
        // Act
        var result = sut.ChangeDetails(new string('a', DomainConstants.Product.NameMaxLength + 1), "longDescription", (decimal)1.1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeDetails.NameLongerThanMaxLength);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_ChangeDetails_When_DescriptionIsNullOrEmpty_Then_ShouldFail(string description)
    {
        // Act
        var result = sut.ChangeDetails("name", description, (decimal)1.1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeDetails.DescriptionNullOrEmpty);
    }

    [Fact]
    public void Given_ChangeDetails_When_DescriptionIsShorterThanMin_Then_ShouldFail()
    {
        // Act
        var result = sut.ChangeDetails("name", new string('a', DomainConstants.Product.DescriptionMinLength - 1), (decimal)1.1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeDetails.DescriptionShorterThanMinLength);
    }

    [Fact]
    public void Given_ChangeDetails_When_DescriptionIsLongerThanMax_Then_ShouldFail()
    {
        // Act
        var result = sut.ChangeDetails("name", new string('a', DomainConstants.Product.DescriptionMaxLength + 1), (decimal)1.1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeDetails.DescriptionLongerThanMaxLength);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_Create_When_PriceLessOrEqualToZero_Then_ShouldFail(decimal badPrice)
    {
        // Act
        var result = sut.ChangeDetails("name", "longDescription", badPrice);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeDetails.PriceLessOrEqualToZero);
    }


    [Fact]
    public void Given_ChangeDetails_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var name = "name";
        var description = "description";
        var price = (decimal)1.1;

        // Act
        var result = sut.ChangeDetails(name, description, price);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Name.Should().Be(name);
        sut.Description.Should().Be(description);
        sut.Price.Value.Should().Be(price);
    }
}