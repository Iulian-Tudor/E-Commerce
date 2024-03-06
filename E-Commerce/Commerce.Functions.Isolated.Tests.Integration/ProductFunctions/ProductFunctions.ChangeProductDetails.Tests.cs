using Commerce.Business;
using System.Net.Http.Headers;
using System.Net;
using FluentAssertions;
using System.Text;
using Xunit;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Commerce.Domain;

namespace Commerce.Functions.Isolated.Tests.Integration;

public class ProductFunctionsChangeProductDetailsTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
	[Fact]
	public async Task Given_ChangeProductDetails_When_ProductNotFound_Then_ShouldFail()
	{
		// Arrange
		var id = Guid.NewGuid();
		var requestUri = fixture.Route($"api/v1/products/{id}/details");
		var token = TokensFactory.Any();
		var http = new HttpClient();
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var command = new ChangeProductDetailsCommand(id, "Rosii", "Rosii bune!", 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(BusinessErrors.Product.ChangeDetails.ProductNotFound);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_ProductDoesNotBelongToCaller_Then_ShouldFail()
	{
		// Arrange
		var http = new HttpClient();
		var token = TokensFactory.Any();
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, "Rosii", "Rosii bune!", 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(BusinessErrors.Product.ChangeDetails.ProductDoesNotBelongToCaller);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public async Task Given_ChangeProductDetails_When_NameNullOrEmpty_Then_ShouldFail(string badName)
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, badName , "Rosii bune!", 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.ChangeDetails.NameNullOrEmpty);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_NameTooLong_Then_ShouldFail()
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, string.Join("", Enumerable.Repeat("a", DomainConstants.Product.NameMaxLength + 1)), "Rosii bune!", 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.ChangeDetails.NameLongerThanMaxLength);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_NameTooShort_Then_ShouldFail()
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, string.Join("", Enumerable.Repeat("a", DomainConstants.Product.NameMinLength - 1)), "Rosii bune!", 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.ChangeDetails.NameShorterThanMinLength);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public async Task Given_ChangeProductDetails_When_DescriptionNullOrEmpty_Then_ShouldFail(string badDescription)
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, "Rosii", badDescription, 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.ChangeDetails.DescriptionNullOrEmpty);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_DescriptionTooLong_Then_ShouldFail()
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, "Rosii", string.Join("", Enumerable.Repeat("a", DomainConstants.Product.DescriptionMaxLength + 1)), 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.ChangeDetails.DescriptionLongerThanMaxLength);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_DescriptionTooShort_Then_ShouldFail()
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, "Rosii", string.Join("", Enumerable.Repeat("a", DomainConstants.Product.DescriptionMinLength - 1)), 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.ChangeDetails.DescriptionShorterThanMinLength);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_PriceLessOrEqualToZero_Then_ShouldFail()
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, "Rosii","Rosii bune!", 0);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(DomainErrors.Product.ChangeDetails.PriceLessOrEqualToZero);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_NotViolatingConstraints_Then_ShouldSucceed()
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}/details");

		var command = new ChangeProductDetailsCommand(product.Id, "Rosii", "Rosii bune!", 3);
		var reqAsJson = JsonConvert.SerializeObject(command);

		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);

		var result = await response.DeserializeResponseBody<ApiResult>();

		result.Should().BeNull();
	}

}