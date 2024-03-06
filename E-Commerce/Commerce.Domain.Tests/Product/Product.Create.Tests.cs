using Xunit;
using FluentAssertions;

using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Domain.Tests;

public sealed class ProductCreateTests
{
    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Given_Create_When_NameNullOrEmpty_Then_ShouldFail(string badName)
    {

        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", badName, "longDescription", 3.14m, Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.NameNullOrEmpty);
    }

    [Fact]
    public void Given_Create_When_NameShorterThanMinLength_Then_ShouldFail()
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", new string('a', DomainConstants.Product.NameMinLength - 1), "longDescription", 3.14m, Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.NameShorterThanMinLength);
    }
    [Fact]

    public void Given_Create_When_NameLongerThanMaxLength_Then_ShouldFail()
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", new string('a', DomainConstants.Product.NameMaxLength + 1), "longDescription", 3.14m, Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.NameLongerThanMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Given_Create_When_DescriptionNullOrEmpty_Then_ShouldFail(string badDescription)
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", "name", badDescription, 3.14m, Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.DescriptionNullOrEmpty);
    }

    [Fact]
    public void Given_Create_When_DescriptionShorterThanMinLength_Then_ShouldFail()
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", "name", new string('a', DomainConstants.Product.DescriptionMinLength - 1), 3.14m, Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.DescriptionShorterThanMinLength);
    }

    [Fact]
    public void Given_Create_When_DescriptionLongerThanMaxLength_Then_ShouldFail()
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", "name", new string('a', DomainConstants.Product.DescriptionMaxLength + 1), 3.14m, Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.DescriptionLongerThanMaxLength);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_Create_When_PriceLessOrEqualToZero_Then_ShouldFail(decimal badPrice)
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", "name", "longDescription", badPrice, Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.PriceLessOrEqualToZero);
    }

    [Fact]
    public void Given_Create_When_CategoryIdEmpty_Then_ShouldFail()
    {
        // Act
        var result = Product.Create(Guid.NewGuid(), "vendorName", "name", "longDescription", 3.14m, Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.Create.CategoryIdEmpty);
    }

    [Fact]
    public void Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var vendorName = "vendorName";
        var name = "name";
        var description = "longDescription";
        var price = (decimal)3.14;
        var categoryId = Guid.NewGuid();

        // Act
        var result = Product.Create(vendorId, vendorName, name, description, price, categoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.VendorId.Should().Be(vendorId);
        result.Value.VendorName.Should().Be(vendorName);
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.Price.Value.Should().Be(price);
        result.Value.IsVisible.Should().BeFalse();
        result.Value.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Given_VerboseCreate_Then_ShouldPassThroughCreate_And_ModifyLeftOverProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var vendorId = Guid.NewGuid();
        var vendorName = "vendorName";
        var name = "name";
        var description = "longDescription";
        var price = (decimal)3.14;
        var isVisible = true;
        var categoryId = Guid.NewGuid();
        var mediaAsset = MediaAsset.Create(Guid.NewGuid(), "path", "also a path", TimeProvider.Instance().UtcNow).Value;

        // Act
        var result = Product.Create(id, vendorId, vendorName, name, description, price, categoryId, isVisible, mediaAsset);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.VendorId.Should().Be(vendorId);
        result.Value.VendorName.Should().Be(vendorName);
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.Price.Value.Should().Be(price);
        result.Value.IsVisible.Should().Be(isVisible);
        result.Value.CategoryId.Should().Be(categoryId);
        result.Value.MediaAsset.Value.Should().Be(mediaAsset);
    }
}
