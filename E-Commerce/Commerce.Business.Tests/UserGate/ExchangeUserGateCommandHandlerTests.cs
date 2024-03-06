using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;
using CSharpFunctionalExtensions;

using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Business.Tests;

public sealed class ExchangeUserGateCommandHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IUserGateRepository userGateRepository = Substitute.For<IUserGateRepository>();
    private readonly ITokenService tokenService = Substitute.For<ITokenService>();

    private readonly User user = UsersFactory.Any();
    private readonly UserGate userGate;
    private readonly UserGateReadModel userGateReadModel;

    public ExchangeUserGateCommandHandlerTests()
    {
        userGate = UserGatesFactory.ForUser(user.Id).Passed();
        userGateReadModel = new UserGateReadModel().FromAggregate(userGate);

        userRepository.Load(user.Id).Returns(user);

        userGateRepository.Load(userGate.Id).Returns(userGate);
        userGateRepository.Query().Returns(new List<UserGateReadModel> { userGateReadModel }.AsQueryable());

        tokenService.GenerateAuthToken(user).Returns("token");
        tokenService.GenerateRefreshToken(user).Returns(new Token("token", TimeProvider.Instance().UtcNow));
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
        result.Error.Should().Be(BusinessErrors.UserGate.Exchange.UserNotFound);

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
        result.Error.Should().Be(BusinessErrors.UserGate.Exchange.UserGateNotFound);

        await userGateRepository.DidNotReceive().Store(Arg.Any<UserGate>());
    }

    [Fact]
    public async Task When_TokenServiceFails_Then_ShouldFail()
    {
        // Arrange
        tokenService.GenerateAuthToken(user).Returns(Result.Failure<string>("bad"));

        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("bad");

        await userGateRepository.DidNotReceive().Store(Arg.Any<UserGate>());
    }

    [Fact]
    public async Task When_DomainFails_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { GateSecret = string.Empty };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Exchange.SecretInvalid);

        await userGateRepository.DidNotReceive().Store(Arg.Any<UserGate>());
    }

    [Fact]
    public async Task When_DomainSucceeds_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RefreshToken.Should().NotBeNull();
        result.Value.AuthToken.Should().NotBeNullOrWhiteSpace();

        await userGateRepository.Received(1).Store(Arg.Any<UserGate>());
    }

    private ExchangeUserGateCommand Command() => new(user.Id, userGate.Secret);

    private ExchangeUserGateCommandHandler Sut() => new(userRepository, userGateRepository, tokenService);
}