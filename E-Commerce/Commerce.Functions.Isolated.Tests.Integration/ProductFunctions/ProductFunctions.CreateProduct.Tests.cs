using Xunit;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using FluentAssertions;
using Commerce.Domain;
using Commerce.Business;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Commerce.Functions.Isolated.Tests.Integration;

public sealed class ProductFunctionsCreateProductTests(AzureFunctionsTestContainersFixture fixture): IClassFixture<AzureFunctionsTestContainersFixture>
{
	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public async Task Given_Create_When_ProductNameNullOrEmpty_Then_ShouldFail(string badName)
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var description = "A valid product description";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, badName, description, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.NameNullOrEmpty);
	}

	[Fact]
	public async Task Given_Create_When_ProductNameTooShort_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var description = "A valid product description";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, "a", description, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		//Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.NameShorterThanMinLength);
	}

	[Fact]
	public async Task Given_Create_When_ProductNameTooLong_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var description = "A valid product description";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, string.Join("", Enumerable.Repeat("a", DomainConstants.Product.NameMaxLength + 1)), description, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		//Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.NameLongerThanMaxLength);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public async Task Given_Create_When_DescriptionNullOrEmpty_Then_ShouldFail(string badDescription)
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var name = "A valid product name";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, name, badDescription, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		//Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.DescriptionNullOrEmpty);
	}

	[Fact]
	public async Task Given_Create_When_DescriptionTooShort_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var name = "A valid product name";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, name, "a", price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		//Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.DescriptionShorterThanMinLength);
	}

	[Fact]
	public async Task Given_Create_When_DescriptionTooLong_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var name = "A valid product name";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, name, string.Join("", Enumerable.Repeat("a", DomainConstants.Product.DescriptionMaxLength + 1)), price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		//Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.DescriptionLongerThanMaxLength);
	}

	[Fact]
	public async Task Given_Create_When_PriceLessOrEqualToZero_Then_ShouldFail()
	{
		// Arrange
		var client = new HttpClient();
		var token = TokensFactory.Any();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var name = "A valid product name";
		var description = "A valid product description";
		var price = 0m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, name, description, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.PriceLessOrEqualToZero);
	}

	[Fact]
	public async Task Given_Create_When_CategoryIdEmpty_Then_ShouldFail()
	{
		// Arrange
		var client = new HttpClient();
		var token = TokensFactory.Any();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var name = "A valid product name";
		var description = "A valid product description";
		var price = 9.99m;
		var categoryId = Guid.Empty;
		var command = new CreateProductCommand(vendorId, name, description, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.Create.CategoryIdEmpty);
	}

	[Fact]
	public async Task Given_Create_When_VendorNotFound_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = Guid.Empty;
		var vendorName = "Vendor Name";
		var productName = "New Product";
		var description = "A valid product description";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, productName, description, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(BusinessErrors.Product.Create.UserNotFound);
	}

	[Fact]
	public async Task Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var vendorId = (await client.GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"))).First().Id;
		var vendorName = "Vendor Name";
		var productName = "New Product";
		var description = "A valid product description";
		var price = 9.99m;
		var categoryId = (await client.GetFromJsonAsync<IEnumerable<CategoryReadModel>>(fixture.Route("api/v1/categories"))).First().Id;
		var command = new CreateProductCommand(vendorId, productName, description, price, categoryId);
		var requestUri = fixture.Route("api/v1/products");
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result.Should().BeNull();
	}
}
