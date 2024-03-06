using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class ProductChangeImageTests
{
    private readonly Product sut = ProductsFactory.Any();

    [Fact]
    public void Given_ChangeImage_When_MediaAssetIsNull_Then_ShouldFail()
    {
        // Act
        var result = sut.ChangeImage(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Product.ChangeImage.MediaAssetNull);
    }

    [Fact]
    public void Given_ChangeImage_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var mediaAsset = MediaAssetsFactory.Any();

        // Act
        var result = sut.ChangeImage(mediaAsset);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.MediaAsset.Should().Be(mediaAsset);
    }
}