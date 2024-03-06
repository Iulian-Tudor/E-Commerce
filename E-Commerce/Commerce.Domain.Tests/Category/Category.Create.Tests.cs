using Xunit;
using FluentAssertions;

namespace Commerce.Domain.Tests;

public sealed class CategoryCreateTests
{
	[Theory]
	[InlineData("")]
	[InlineData("    ")]
	[InlineData(null)]
	public void Given_Create_When_NameNullOrEmpty_Then_ShouldFail(string badName)
	{
		// Act
		var result = Category.Create(badName, "description");

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Category.Create.NameNullOrEmpty);
	}

	[Fact]
	public void Given_Create_When_NameLongerThanMax_Then_ShouldFail()
	{
		// Act
		var result = Category.Create(new string('a', DomainConstants.Category.NameMaxLength + 1), "description");

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Category.Create.NameLongerThanMaxLength);
	}

	[Fact]
	public void Given_Create_When_NameShorterThanMin_Then_ShouldFail()
	{
		// Act
		var result = Category.Create(new string('a', DomainConstants.Category.NameMinLength - 1), "description");

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Category.Create.NameShorterThanMinLength);
	}

	[Theory]
	[InlineData("")]
	[InlineData("    ")]
	[InlineData(null)]
	public void Given_Create_When_DescriptionNullOrEmpty_Then_ShouldFail(string badDescription)
	{
		// Act
		var result = Category.Create("name", badDescription);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Category.Create.DescriptionNullOrEmpty);
	}

	[Fact]
	public void Given_Create_When_DescriptionLongerThanMax_Then_ShouldFail()
	{
		// Act
		var result = Category.Create("name", new string('a', DomainConstants.Category.DescriptionMaxLength + 1));

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Category.Create.DescriptionLongerThanMaxLength);
	}

	[Fact]
	public void Given_Create_When_DescriptionShorterThanMin_Then_ShouldFail()
	{
		// Act
		var result = Category.Create("name", new string('a', DomainConstants.Category.DescriptionMinLength - 1));

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Category.Create.DescriptionShorterThanMinLength);
	}

	[Fact]
	public void Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
	{
		// Arrange
		var name = "name";
		var description = "description";

		// Act
		var result = Category.Create(name, description);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Name.Should().Be(name);
		result.Value.Description.Should().Be(description);
	}

	[Fact]
	public void Given_VerboseCreate_Then_ShouldPassThroughCreate_And_ModifyLeftOverProperties()
	{
		// Arrange
		var id = Guid.NewGuid();
		var name = "name";
		var description = "description";

		// Act
		var result = Category.Create(id, name, description);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Id.Should().Be(id);
		result.Value.Name.Should().Be(name);
		result.Value.Description.Should().Be(description);
	}
}
