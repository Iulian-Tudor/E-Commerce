using Xunit;
using FluentAssertions;
using Commerce.SharedKernel.Domain;

namespace Commerce.Domain.Tests;

public sealed class UserGateExchangeTests
{
    private UserGate sut = UserGatesFactory.Any().Passed();

    [Fact]
    public void Given_Exchange_When_GateAlreadyExchanged_Then_ShouldFail()
    {
        // Arrange
        sut.Exchanged();

        // Act
        var result = sut.Exchange(sut.Secret);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Exchange.UserGateAlreadyExchanged);
    }

    [Fact]
    public void Given_Exchange_When_GateNotPassed_Then_ShouldFail()
    {
        // Arrange
        sut = UserGatesFactory.Any();

        // Act
        var result = sut.Exchange(sut.Secret);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Exchange.UserGateNotPassed);
    }

    [Fact]
    public void Given_Exchange_When_ExchangeWindowExpired_Then_ShouldFail()
    {
        // Arrange
        TimeProviderContext.AdvanceTimeTo(sut.ExchangeWindowExpiresAt.Value);

        // Act
        var result = sut.Exchange(sut.Secret);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Exchange.UserGateExpired);
    }

    [Fact]
    public void Given_Exchange_When_SecretInvalid_Then_ShouldFail()
    {
        // Act
        var result = sut.Exchange("invalid");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Exchange.SecretInvalid);
    }

    [Fact]
    public void Given_Exchange_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var now = TimeProviderContext.AdvanceTimeTo(sut.PassedAt.Value);

        // Act
        var result = sut.Exchange(sut.Secret);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.ExchangedAt.Should().Be(now);
    }
}