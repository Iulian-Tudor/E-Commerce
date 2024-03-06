using Xunit;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using FluentAssertions;
using Commerce.Domain;
using Commerce.Business;
using System.Net.Http.Headers;

namespace Commerce.Functions.Isolated.Tests.Integration;

public sealed class CategoryFunctionsCreateCategoryTests(AzureFunctionsTestContainersFixture fixture): IClassFixture<AzureFunctionsTestContainersFixture>
{
	[Fact]
	public async Task Given_Create_When_CategoryNameAlreadyExists_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var requestUri = fixture.Route("api/v1/categories");
		var getResponse = await new HttpClient().GetAsync(requestUri);
		var categories = await getResponse.DeserializeResponseBody<IEnumerable<CategoryReadModel>>();
		var command = new CreateCategoryCommand(categories.First().Name, "Some description");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(BusinessErrors.Category.Create.CategoryAlreadyExists);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("    ")]
	public async Task Given_Create_When_NameNullOrEmpty_Then_ShouldFail(string badName)
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var command = new CreateCategoryCommand(badName, "Valid description");
		var requestUri = fixture.Route("api/v1/categories");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Category.Create.NameNullOrEmpty);
	}

	[Fact]
	public async Task Given_Create_When_NameLongerThanMaxLength_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var command = new CreateCategoryCommand(new string('a', DomainConstants.Category.NameMaxLength + 1), "Valid description");
		var requestUri = fixture.Route("api/v1/categories");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Category.Create.NameLongerThanMaxLength);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("    ")]
	public async Task Given_Create_When_DescriptionNullOrEmpty_Then_ShouldFail(string badDescription)
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var command = new CreateCategoryCommand("Valid name", badDescription);
		var requestUri = fixture.Route("api/v1/categories");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Category.Create.DescriptionNullOrEmpty);
	}

	[Fact]
	public async Task Given_Create_When_DescriptionLongerThanMaxLength_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var command = new CreateCategoryCommand("Valid name", new string('a', DomainConstants.Category.DescriptionMaxLength + 1));
		var requestUri = fixture.Route("api/v1/categories");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Category.Create.DescriptionLongerThanMaxLength);
	}

	[Fact]
	public async Task Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var command = new CreateCategoryCommand("uniqueCategoryName", "A valid description");
		var requestUri = fixture.Route("api/v1/categories");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		var result = await response.DeserializeResponseBody<CategoryReadModel>();
		result.Should().BeNull();
	}
}
