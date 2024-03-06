using Xunit;
using NSubstitute;
using Commerce.Domain;
using FluentAssertions;

namespace Commerce.Business.Tests;

public sealed class CreateCategoryCommandHandlerTests
{
	private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();

	public CreateCategoryCommandHandlerTests()
	{
		categoryRepository.Query().Returns(new List<CategoryReadModel>().AsQueryable());
	}

	[Fact]
	public async Task When_CategoryAlreadyExists_Then_ShouldFail()
	{
		// Arrange
		var existingCategory = new CategoryReadModel { Id = Guid.NewGuid(), Name = "Existing Category", Description = "Existing Description" };
		categoryRepository.Query().Returns(new List<CategoryReadModel> { existingCategory }.AsQueryable());

		var command = Command() with { Name = existingCategory.Name };

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(BusinessErrors.Category.Create.CategoryAlreadyExists);

		await categoryRepository.DidNotReceive().Store(Arg.Any<Category>());
	}

	[Fact]
	public async Task When_DomainFails_Then_ShouldFail()
	{
		// Arrange
		var command = Command() with { Name = string.Empty };

		// Act
		var result = await Sut().Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Should().Be(DomainErrors.Category.Create.NameNullOrEmpty);

		await categoryRepository.DidNotReceive().Store(Arg.Any<Category>());
	}

	[Fact]
	public async Task When_DomainSucceeds_Then_ShouldSucceed()
	{
		// Act
		var result = await Sut().Handle(Command(), CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();

		await categoryRepository.Received(1).Store(Arg.Any<Category>());
	}

	private CreateCategoryCommand Command() => new("Category Name", "Category Description");

	private CreateCategoryCommandHandler Sut() => new(categoryRepository);
}
