using Xunit;
using System.Net;
using System.Text;
using Commerce.Domain;
using FluentAssertions;
using Commerce.Business;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Commerce.Functions.Isolated.Tests.Integration;

public sealed class ProductFunctionsMakeProductVisibleTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
    [Fact]
    public async Task Given_MakeProductVisible_When_ProductNotFound_Then_ShouldFail()
    {
        // Arrange
        var id = Guid.NewGuid();
        var requestUri = fixture.Route($"api/v1/products/{id}/visibility");
        var token = TokensFactory.Any();
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await http.PatchAsync(requestUri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.Product.MakeVisible.ProductNotFound);
    }

    [Fact]
    public async Task Given_MakeProductVisible_When_ProductDoesNotBelongToCaller_Then_ShouldFail()
    {
        // Arrange
        var http = new HttpClient();
        var token = TokensFactory.Any();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productUri = fixture.Route("api/v1/products");
        var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(productUri);
        var product = products.First();

        var requestUri = fixture.Route($"api/v1/products/{product.Id}/visibility");

        // Act
        var response = await http.PatchAsync(requestUri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.Product.MakeVisible.ProductDoesNotBelongToCaller);
    }

    [Fact]
    public async Task Given_MakeProductVisible_When_ProductAlreadyVisible_Then_ShouldFail()
    {
        // Arrange
        var http = new HttpClient();

        var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(fixture.Route("api/v1/products"));
        var product = products.First();

        var token = TokensFactory.WithId(product.VendorId);
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var makeVisibleUri = fixture.Route($"api/v1/products/{product.Id}/visibility");
        await http.PatchAsync(makeVisibleUri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        // Act
        var response = await http.PatchAsync(makeVisibleUri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.Product.MakeVisible.ProductAlreadyVisible);
    }

    [Fact]
    public async Task Given_MakeProductVisible_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var http = new HttpClient();

        var products = await http.GetFromJsonAsync<IEnumerable<ProductReadModel>>(fixture.Route("api/v1/products"));
        var product = products.Last();

        var token = TokensFactory.WithId(product.VendorId);
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var makeVisibleUri = fixture.Route($"api/v1/products/{product.Id}/visibility");

        // Act
        var response = await http.PatchAsync(makeVisibleUri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}