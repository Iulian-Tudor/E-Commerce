using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class UserCreateTests
{
    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Given_Create_When_FirstNameNullOrEmpty_Then_ShouldFail(string badFirstName)
    {
        // Act
        var result = User.Create(badFirstName, "lastName", "email");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.FirstNameNullOrEmpty);
    }

    [Fact]
    public void Given_Create_When_FirstNameLongerThanMax_Then_ShouldFail()
    {
        // Act
        var result = User.Create(new string('a', DomainConstants.User.FirstNameMaxLength + 1), "lastName", "email");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.FirstNameLongerThanMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Given_Create_When_LastNameNullOrEmpty_Then_ShouldFail(string badLastName)
    {
        // Act
        var result = User.Create("firstName", badLastName, "email");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.LastNameNullOrEmpty);
    }

    [Fact]
    public void Given_Create_When_LastNameLongerThanMax_Then_ShouldFail()
    {
        // Act
        var result = User.Create("firstName", new string('a', DomainConstants.User.LastNameMaxLength + 1), "email");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.LastNameLongerThanMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Given_Create_When_EmailNullOrEmpty_Then_ShouldFail(string badEmail)
    {
        // Act
        var result = User.Create("firstName", "lastName", badEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.EmailAddressNullOrEmpty);
    }

    [Fact]
    public void Given_Create_When_EmailLongerThanMax_Then_ShouldFail()
    {
        // Act
        var result = User.Create("firstName", "lastName", new string('a', DomainConstants.User.EmailAddressMaxLength + 1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.EmailAddressLongerThanMaxLength);
    }

    [Theory]
    [InlineData("bademail")]
    [InlineData("bademail@")]
    [InlineData("bademail@bad")]
    [InlineData("bademail@bad.")]
    [InlineData("bademail@bad.c")]
    [InlineData("gmail.com")]
    [InlineData("@gmail.com")]
    [InlineData("bademail@gmail")]
    public void Given_Create_When_EmailInvalid_Then_ShouldFail(string badEmail)
    {
        // Act
        var result = User.Create("firstName", "lastName", badEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Create.EmailAddressInvalidFormat);
    }

    [Fact]
    public void Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var firstName = "firstName";
        var lastName = "lastName";
        var email = "test@mail.com";

        // Act
        var result = User.Create(firstName, lastName, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.EmailAddress.Should().Be(email);
    }

    [Fact]
    public void Given_VerboseCreate_Then_ShouldPassThroughCreate_And_ModifyLeftOverProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "firstName";
        var lastName = "lastName";
        var email = "test@mail.com";

        // Act
        var result = User.Create(id, firstName, lastName, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.EmailAddress.Should().Be(email);
    }
}