using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;
using CSharpFunctionalExtensions;

namespace Commerce.Business.Tests;

public sealed class CreateUserGateCommandHandlerTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IUserGateRepository userGateRepository = Substitute.For<IUserGateRepository>();
    private readonly IEmailService emailService = Substitute.For<IEmailService>();

    private readonly UserReadModel user = new UserReadModel().FromAggregate(UsersFactory.Any());

    public CreateUserGateCommandHandlerTests()
    {
        userRepository.Query().Returns(new List<UserReadModel> { user }.AsQueryable());
    }

    [Fact]
    public async Task When_UserNotFound_Then_ShouldFail()
    {
        // Arrange
        var command = Command() with { EmailAddress = Guid.NewGuid().ToString() };

        // Act
        var result = await Sut().Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.UserGate.Create.UserNotFound);

        await userGateRepository.DidNotReceive().Store(Arg.Any<UserGate>());
    }

    [Fact]
    public async Task When_DomainSucceeds_Then_ShouldSucceed()
    {
        // Act
        var result = await Sut().Handle(Command(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await userGateRepository.Received(1).Store(Arg.Any<UserGate>());
    }

    private CreateUserGateCommand Command() => new(user.EmailAddress);

    private CreateUserGateCommandHandler Sut() => new(userRepository, userGateRepository, emailService);
}