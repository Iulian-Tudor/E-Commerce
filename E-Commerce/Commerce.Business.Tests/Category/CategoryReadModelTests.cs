using Xunit;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class CategoryReadModelTests
{
	[Fact]
	public void Given_ToAggregate_Then_ShouldReturnCorrectCategory()
	{
		// Arrange
		var id = Guid.NewGuid();
		var name = "name";
		var description = "description";
		var readModel = new CategoryReadModel { Id = id, Name = name, Description = description };

		// Act
		var category = readModel.ToAggregate();

		// Assert
		category.Id.Should().Be(id);
		category.Name.Should().Be(name);
		category.Description.Should().Be(description);
	}

	[Fact]
	public void Given_FromAggregate_Then_ShouldReturnCorrectReadModel()
	{
		// Arrange
		var id = Guid.NewGuid();
		var name = "name";
		var description = "description";
		var category = Category.Create(id, name, description).Value;

		// Act
		var readModel = new CategoryReadModel().FromAggregate(category);

		// Assert
		readModel.Id.Should().Be(id);
		readModel.Name.Should().Be(name);
		readModel.Description.Should().Be(description);
	}
}