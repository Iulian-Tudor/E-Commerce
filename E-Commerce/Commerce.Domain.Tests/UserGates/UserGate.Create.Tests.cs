using Xunit;
using FluentAssertions;
using Commerce.SharedKernel.Domain;

using TimeProvider = Commerce.SharedKernel.Domain.TimeProvider;

namespace Commerce.Domain.Tests;

public sealed class UserGateCreateTests
{
    [Fact]
    public void Given_Create_When_UserIdEmpty_Then_ShouldFail()
    {
        // Act
        var result = UserGate.Create(Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Create.UserIdEmpty);
    }

    [Fact]
    public void Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var now = TimeProviderContext.AdvanceTimeToNow();

        // Act
        var result = UserGate.Create(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.CreatedAt.Should().Be(now);
    }

    [Fact]
    public void Given_VerboseCreate_Then_ShouldPassThroughCreate_And_ModifyLeftOverProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var passCode = "123456";
        var secret = "secret";
        var createdAt = TimeProvider.Instance().UtcNow;
        var passedAt = TimeProvider.Instance().UtcNow;
        var exchangedAt = TimeProvider.Instance().UtcNow;

        // Act
        var result = UserGate.Create(id, userId, passCode, secret, createdAt, passedAt, exchangedAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.UserId.Should().Be(userId);
        result.Value.PassCode.Should().Be(passCode);
        result.Value.Secret.Should().Be(secret);
        result.Value.CreatedAt.Should().Be(createdAt);
        result.Value.PassedAt.Should().Be(passedAt);
        result.Value.ExchangedAt.Should().Be(exchangedAt);
    }
}