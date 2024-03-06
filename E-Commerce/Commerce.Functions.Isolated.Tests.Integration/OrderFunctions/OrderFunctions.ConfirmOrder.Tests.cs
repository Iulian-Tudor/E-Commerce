using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Commerce.Business;
using FluentAssertions;
using Xunit;

namespace Commerce.Functions.Isolated.Tests.Integration;

public sealed class OrderFunctionsConfirmOrderTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
	[Fact]
	public async Task Given_ConfirmOrder_When_OrderNotFound_Then_ShouldFail()
	{
		// Arrange
		var orderId = Guid.NewGuid();
		var orderedProductId = Guid.NewGuid();
		var requestUri = fixture.Route($"api/v1/orders/{orderId}/products/{orderedProductId}/confirmation");
		var token = TokensFactory.Any();
		var http = new HttpClient();
		http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		//Act
		var response =
			await http.PatchAsync(requestUri, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

		//Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

		var result = await response.DeserializeResponseBody<ApiResult>();
		result!.IsSuccess.Should().BeFalse();
		result!.ErrorCode.Should().Be(BusinessErrors.OrderedProduct.Confirm.OrderNotFound);
	}
}