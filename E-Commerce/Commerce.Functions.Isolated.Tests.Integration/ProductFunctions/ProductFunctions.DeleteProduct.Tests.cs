using Xunit;
using System.Net;
using FluentAssertions;
using Commerce.Business;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Commerce.Functions.Isolated.Tests.Integration;

public class ProductFunctionsDeleteProductTests(AzureFunctionsTestContainersFixture fixture): IClassFixture<AzureFunctionsTestContainersFixture>
{
	[Fact]
	public async Task Given_Delete_When_ProductDoesNotExist_Then_ShouldFail()
	{
		// Arrange
		var client = new HttpClient();
		var token = TokensFactory.Any();
		var requestUri = fixture.Route("api/v1/products");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var productId = Guid.NewGuid();

		// Act
		var response = await client.DeleteAsync($"{requestUri}/{productId}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(BusinessErrors.Product.Delete.ProductNotFound);
	}

	[Fact]
	public async Task Given_Delete_When_ProductDoesNotBelongToCaller_Then_ShouldFail()
	{
		// Arrange
		var token = TokensFactory.Any();
		var client = new HttpClient();
		var productsUri = fixture.Route("api/v1/products");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		var products = await client.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productsUri);
		var productId = products.First().Id;
		
		var otherToken = TokensFactory.Any();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherToken);

		// Act
		var response = await client.DeleteAsync($"{productsUri}/{productId}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(BusinessErrors.Product.Delete.ProductDoesNotBelongToCaller);
	}

	[Fact]
	public async Task Given_ChangeProductDetails_When_NotViolatingConstraints_Then_ShouldSucceed()
	{
		// Arrange
		var http = new HttpClient();

		var productUri = fixture.Route("api/v1/products");
		var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
		var product = products.First();

		var requestUri = fixture.Route($"api/v1/products/{product.Id}");


		var token = TokensFactory.WithId(product.VendorId);
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		// Act
		var response = await http.DeleteAsync(requestUri);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		var result = await response.DeserializeResponseBody<ApiResult>();
		result.Should().BeNull();
	}
}