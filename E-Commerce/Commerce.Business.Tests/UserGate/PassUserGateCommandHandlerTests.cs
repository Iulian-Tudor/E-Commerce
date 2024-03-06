using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;
using CSharpFunctionalExtensions;

namespace Commerce.Business.Tests;

public sealed class PassUserGateCommandHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IUserGateRepository userGateRepository = Substitute.For<IUserGateRepository>();

    private readonly User user = UsersFactory.Any();
    private readonly UserGate userGate;
    private readonly UserGateReadModel userGateReadModel;

    public PassUserGateCommandHandlerTests()
    {
        userGate = UserGatesFactory.ForUser(user.Id);
        userGateReadModel = new UserGateReadModel().FromAggregate(userGate);

        userRepository.Load(user.Id).Returns(user);
        userGateRepository.Load(userGate.Id).Returns(userGate);
        userGateRepository.Query().Returns(new List<UserGateReadModel> { userGateReadModel }.AsQueryable());
    }

    [Fact]
    public async Task When_UserNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { UserId = Guid.NewGuid() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.UserGate.Pass.UserNotFound);

        await userGateRepository.DidNotReceive().Store(Arg.Any<UserGate>());
    }

    [Fact]
    public async Task When_UserGateNotFound_Then_ShouldFail()
    {
        // Arrange
        userGateRepository.Load(userGate.Id).Returns(Maybe<UserGate>.None);

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.UserGate.Pass.UserGateNotFound);

        await userGateRepository.DidNotReceive().Store(Arg.Any<UserGate>());
    }

    [Fact]
    public async Task When_DomainFails_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { PassCode = string.Empty };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Pass.PassCodeInvalid);

        await userGateRepository.DidNotReceive().Store(Arg.Any<UserGate>());
    }

    [Fact]
    public async Task When_DomainSucceeds_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.GateSecret.Should().Be(userGate.Secret);

        await userGateRepository.Received(1).Store(Arg.Any<UserGate>());
    }

    private PassUserGateCommand Command() => new(user.Id, userGate.PassCode);

    private PassUserGateCommandHandler Sut() => new(userRepository, userGateRepository);
}