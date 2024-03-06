using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Commerce.Business;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Commerce.Functions.Isolated.Tests.Integration;

public class ProductFunctionsAddProductToFavoritesTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
    [Fact]
    public async Task Given_Add_When_ProductNotFound_Then_Should_Fail()
    {
        //Arrange
        var client = new HttpClient();
        var token = TokensFactory.Any();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var productId = Guid.NewGuid();

        var command = new AddProductToFavoritesCommand(productId);
        var reqAsJson = JsonConvert.SerializeObject(command);
        var requestUri = fixture.Route("api/v1/favorite-products");

        //Act
        var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.FavoriteProductSnapshot.Create.ProductNotFound);

    }

    [Fact]
    public async Task Given_Add_When_ProductAlreadyInFavorites_Then_Should_Fail()
    {
        //Arrange
        var client = new HttpClient();
        var token = TokensFactory.Any();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productUri = fixture.Route("api/v1/products");
        var products = await client.GetFromJsonAsync<List<ProductReadModel>>(productUri);
        var product = products.First();

        var favoriteProductsRequest = new AddProductToFavoritesCommand(product.Id);
        
        var favoriteProductUri = fixture.Route("api/v1/favorite-products");

        var reqAddFavAsJson = JsonConvert.SerializeObject(favoriteProductsRequest);
        await client.PostAsync(favoriteProductUri, new StringContent(reqAddFavAsJson, Encoding.UTF8, "application/json"));

        var command = new AddProductToFavoritesCommand(product.Id);
        var reqAsJson = JsonConvert.SerializeObject(command);

        //Act
        var response = await client.PostAsync(favoriteProductUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.FavoriteProductSnapshot.Create.ProductAlreadyInFavorites);
    }

    [Fact]
    public async Task Given_Add_When_NotViolatingConstraints_Then_Should_Succeed()
    {
        //Arrange
        var client = new HttpClient();
        var token = TokensFactory.Any();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productUri = fixture.Route("api/v1/products");
        var products = await client.GetFromJsonAsync<List<ProductReadModel>>(productUri);
        var product = products.First();

        var requestUri = fixture.Route("api/v1/favorite-products");

        var command = new AddProductToFavoritesCommand(product.Id);
        var reqAsJson = JsonConvert.SerializeObject(command);

        //Act
        var response = await client.PostAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var result = await response.DeserializeResponseBody<ApiResult>();

        result.Should().BeNull();
    }
}