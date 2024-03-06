using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Business.Tests;

public sealed class DeleteFavoriteProductCommandHandlerTests
{

    private readonly IFavoriteProductSnapshotRepository favoriteProductSnapshotRepository = Substitute.For<IFavoriteProductSnapshotRepository>();
    private IIdentifiedUser identifiedUser = TestableUser.Any();

    private readonly FavoriteProductSnapshot product;

    public DeleteFavoriteProductCommandHandlerTests()
    {
        product = FavoriteProductSnapshot.Create(Guid.NewGuid(), identifiedUser.Id, Guid.NewGuid(), 10m, TimeProvider.Instance().UtcNow).Value;
        favoriteProductSnapshotRepository
            .Load(product.Id)
            .Returns(product);
    }

    [Fact]
    public async void When_ProductNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { FavoriteProductSnapshotId = Guid.NewGuid() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.FavoriteProductSnapshot.Delete.FavoriteProductSnapshotNotFound);

        await favoriteProductSnapshotRepository
            .DidNotReceive()
            .Delete(Arg.Any<Guid>());
    }

    [Fact]
    public async void When_ProductDoesNotBelongToCaller_Then_ShouldFail()
    {
        // Arrange
        identifiedUser = TestableUser.Any();

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue(); 
        result.Error.Should().Be(BusinessErrors.FavoriteProductSnapshot.Delete.FavoriteProductSnapshotNotOwnedByCaller);

        await favoriteProductSnapshotRepository
            .DidNotReceive()
            .Delete(Arg.Any<Guid>());
    }

    [Fact]
    public async void When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await favoriteProductSnapshotRepository
            .Received(1)
            .Delete(product.Id);
    }

    private DeleteFavoriteProductCommand Command() => new(product.Id);

    private DeleteFavoriteProductCommandHandler Sut() => new(favoriteProductSnapshotRepository, identifiedUser);
}