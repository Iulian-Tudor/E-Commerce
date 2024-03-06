using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Commerce.Business;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Commerce.Functions.Isolated.Tests.Integration;

public class ProductFunctionsRemoveProductFromFavoritesTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
    [Fact]
    public async Task Given_Remove_When_ProductNotFound_Then_Should_Fail()
    {
        //Arrange
        var client = new HttpClient();
        var token = TokensFactory.Any();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var productId = Guid.NewGuid();

        var requestUri = fixture.Route($"api/v1/favorite-products/{productId}");

        //Act
        var response = await client.DeleteAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.FavoriteProductSnapshot.Delete.FavoriteProductSnapshotNotFound);

    }

    [Fact]
    public async Task Given_Remove_When_ProductNotOwnedByCaller_Then_Should_Fail()
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

        var favoriteProducts = await client.GetFromJsonAsync<List<FavoriteProductSnapshotReadModel>>(favoriteProductUri);
        var favoriteProduct = favoriteProducts.First();

        token = TokensFactory.Any();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var requestUri = fixture.Route($"api/v1/favorite-products/{favoriteProduct.Id}");

        //Act
        var response = await client.DeleteAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.FavoriteProductSnapshot.Delete.FavoriteProductSnapshotNotOwnedByCaller);
    }

    [Fact]
    public async Task Given_Remove_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var client = new HttpClient();
        var token = TokensFactory.WithId(userId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productUri = fixture.Route("api/v1/products");
        var products = await client.GetFromJsonAsync<List<ProductReadModel>>(productUri);
        var product = products.First();

        var favoriteProductsRequest = new AddProductToFavoritesCommand(product.Id);

        var favoriteProductUri = fixture.Route("api/v1/favorite-products");

        var reqAddFavAsJson = JsonConvert.SerializeObject(favoriteProductsRequest);
        await client.PostAsync(favoriteProductUri, new StringContent(reqAddFavAsJson, Encoding.UTF8, "application/json"));

        var favoriteProducts = await client.GetFromJsonAsync<List<FavoriteProductSnapshotReadModel>>(favoriteProductUri);
        var favoriteProduct = favoriteProducts.First();

        var requestUri = fixture.Route($"api/v1/favorite-products/{favoriteProduct.Id}");

        //Act
        var response = await client.DeleteAsync(requestUri);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var result = await response.DeserializeResponseBody<ApiResult>();
        result.Should().BeNull();
    }
}
