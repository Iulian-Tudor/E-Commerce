using Xunit;
using FluentAssertions;
using Commerce.SharedKernel.Domain;
using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Domain.Tests;

public sealed class FavoriteProductSnapshotTests
{
    [Fact]
    public void Given_Create_When_UserIdEmpty_Then_ShouldFail()
    {
        // Act
        var result = FavoriteProductSnapshot.Create(Guid.Empty, ProductsFactory.Any());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.FavoriteProductSnapshot.Create.UserIdEmpty);
    }

    [Fact]
    public void Given_Create_When_ProductNull_Then_ShouldFail()
    {
        // Act
        var result = FavoriteProductSnapshot.Create(Guid.NewGuid(), null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.FavoriteProductSnapshot.Create.ProductNull);
    }

    [Fact]
    public void Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var product = ProductsFactory.Any();
        var now = TimeProviderContext.AdvanceTimeToNow();

        // Act
        var result = FavoriteProductSnapshot.Create(userId, product);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.UserId.Should().Be(userId);
        result.Value.ProductId.Should().Be(product.Id);
        result.Value.InitialPrice.Should().Be(product.Price);
        result.Value.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void Given_ExplicitCreate_Then_ShouldCreate()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var initialPrice = new FixedPrecisionPrice(10m);
        var createdAt = TimeProvider.Instance().UtcNow;

        // Act
        var result = FavoriteProductSnapshot.Create(id, userId, productId, initialPrice, createdAt);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Id.Should().Be(id);
        result.Value.UserId.Should().Be(userId);
        result.Value.ProductId.Should().Be(productId);
        result.Value.InitialPrice.Should().Be(initialPrice);
        result.Value.CreatedAt.Should().Be(createdAt);
    }
}