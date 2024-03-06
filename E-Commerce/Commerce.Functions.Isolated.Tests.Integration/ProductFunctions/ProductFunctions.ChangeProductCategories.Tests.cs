using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Commerce.Business;
using Xunit;
using Newtonsoft.Json;
using FluentAssertions;


namespace Commerce.Functions.Isolated.Tests.Integration;

public class ProductFunctionsChangeProductCategoriesTests(AzureFunctionsTestContainersFixture fixture)
    : IClassFixture<AzureFunctionsTestContainersFixture>
{
    [Fact]
    public async Task Given_ChangeProductCategories_When_CategoryNotFound_Then_ShouldFail()
    {
        // Arrange
        var token = TokensFactory.Any();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoryId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();

        var requestUri = fixture.Route($"api/v1/products/{productId}/categories");

        var command = new ChangeProductCategoryCommand(productId, categoryId, vendorId,"Produs","Descriere produs",9m);
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await client.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
         
        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.Product.ChangeCategory.CategoryNotFound);
    }

    [Fact]
    public async Task Given_ChangeProductCategories_When_ProductNotFound_Then_ShouldFail()
    {
        // Arrange
        var token = TokensFactory.Any();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoryUri = fixture.Route("api/v1/categories");
        var categories = await client.GetFromJsonAsync<List<CategoryReadModel>>(categoryUri);
        var category = categories.First();
        var productId = Guid.NewGuid();
        var vendorId = Guid.NewGuid();

        var requestUri = fixture.Route($"api/v1/products/{productId}/categories");

        var command = new ChangeProductCategoryCommand(productId, category.Id, vendorId, "Product", "Product Description",9m );
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await client.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.Product.ChangeCategory.ProductNotFound);
    }

    [Fact]
    public async Task Given_ChangeProductCategories_When_ProductDoesNotBelongToCaller_Then_ShouldFail()
    {
        // Arrange
        var token = TokensFactory.Any();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoryUri = fixture.Route("api/v1/categories");
        var categories = await client.GetFromJsonAsync<List<CategoryReadModel>>(categoryUri);
        var category = categories.First();
        var productUri = fixture.Route("api/v1/products");
        var products = await client.GetFromJsonAsync<List<ProductReadModel>>(productUri);
        var product = products.First();
        var vendorId = Guid.NewGuid();

        var requestUri = fixture.Route($"api/v1/products/{product.Id}/categories");

        var command = new ChangeProductCategoryCommand(product.Id, category.Id, vendorId, product.Name, product.Description, product.Price );
        var reqAsJson = JsonConvert.SerializeObject(command);

        // Act
        var response = await client.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.Product.ChangeCategory.ProductDoesNotBelongToCaller);
    }

    [Fact]
    public async Task Given_ChangeProductCategory_When_NotViolatingConstraints_Then_Should_Succeed()
    {
        // Arrange
        var client = new HttpClient();

        var productUri = fixture.Route("api/v1/products");
        var products = await client.GetFromJsonAsync<List<ProductReadModel>>(productUri);
        var product = products.First();

        var token = TokensFactory.WithId(product.VendorId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var categoryUri = fixture.Route("api/v1/categories");
        var categories = await client.GetFromJsonAsync<List<CategoryReadModel>>(categoryUri);
        var category = categories.Last();

        var requestUri = fixture.Route($"api/v1/products/{product.Id}/categories");

        var command = new ChangeProductCategoryCommand(product.Id, category.Id, product.VendorId, product.Name, product.Description, product.Price );
        var reqAsJson = JsonConvert.SerializeObject(command);  

        // Act
        var response = await client.PatchAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var result = await response.DeserializeResponseBody<ApiResult>();

        result.Should().BeNull();
    }
}