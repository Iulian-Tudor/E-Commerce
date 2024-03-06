using Commerce.Domain;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Commerce.Business.Tests;

public sealed class AddProductToFavoritesCommandHandlerTests
{
    private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();
    private readonly IFavoriteProductSnapshotRepository favoriteProductSnapshotRepository = Substitute.For<IFavoriteProductSnapshotRepository>();
    private readonly IIdentifiedUser identifiedUser = TestableUser.Any();

    private readonly Product product;

    public AddProductToFavoritesCommandHandlerTests()
    {
        product = ProductsFactory.WithId(identifiedUser.Id);

        productRepository
            .Load(product.Id)
            .Returns(product);

        favoriteProductSnapshotRepository
            .Query()
            .Returns(new List<FavoriteProductSnapshotReadModel>().AsQueryable());
    }

    [Fact]
    public async void When_ProductNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { ProductId = Guid.NewGuid() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.FavoriteProductSnapshot.Create.ProductNotFound);

        await favoriteProductSnapshotRepository
            .DidNotReceive()
            .Store(Arg.Any<FavoriteProductSnapshot>());
    }

    [Fact]
    public async void When_ProductAlreadyInFavorites_Then_ShouldFail()
    {
        // Arrange
        var favoriteProductSnapshot = FavoriteProductSnapshot.Create(identifiedUser.Id, product).Value;
        favoriteProductSnapshotRepository
            .Query()
            .Returns(new List<FavoriteProductSnapshotReadModel> { new FavoriteProductSnapshotReadModel().FromAggregate(favoriteProductSnapshot) }.AsQueryable());

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.FavoriteProductSnapshot.Create.ProductAlreadyInFavorites);

        await favoriteProductSnapshotRepository
            .DidNotReceive()
            .Store(Arg.Any<FavoriteProductSnapshot>());
    }

    [Fact]
    public async void When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await favoriteProductSnapshotRepository
            .Received()
            .Store(Arg.Any<FavoriteProductSnapshot>());
    }

    private AddProductToFavoritesCommand Command() => new(product.Id);

    private AddProductToFavoritesCommandHandler Sut() => new(productRepository, favoriteProductSnapshotRepository, identifiedUser);
}