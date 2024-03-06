using Xunit;
using FluentAssertions;
using Commerce.SharedKernel.Domain;

namespace Commerce.Domain.Tests;

public sealed class UserGatePassTests
{
    private readonly UserGate sut = UserGatesFactory.Any();

    [Fact]
    public void Given_Pass_When_GateAlreadyPassed_Then_ShouldFail()
    {
        // Arrange
        sut.Passed();

        // Act
        var result = sut.Pass(sut.PassCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Pass.UserGateAlreadyPassed);
    }

    [Fact]
    public void Given_Pass_When_UserGateExpired_Then_ShouldFail()
    {
        // Arrange
        TimeProviderContext.AdvanceTimeTo(sut.ClosesAt);

        // Act
        var result = sut.Pass(sut.PassCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Pass.UserGateExpired);
    }

    [Fact]
    public void Given_Pass_When_PassCodeInvalid_Then_ShouldFail()
    {
        // Act
        var result = sut.Pass("totally bad pass code");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.UserGate.Pass.PassCodeInvalid);
    }

    [Fact]
    public void Given_Pass_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var now = TimeProviderContext.AdvanceTimeTo(sut.CreatedAt);

        // Act
        var result = sut.Pass(sut.PassCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.PassedAt.Should().Be(now);
    }
}