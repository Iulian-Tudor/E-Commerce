using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Commerce.Business;
using FluentAssertions;
using Xunit;

namespace Commerce.Functions.Isolated.Tests.Integration;

public class UserFunctionsDeleteUserTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
    [Fact]
    public async Task Given_Delete_When_UserNotFound_Then_ShouldFail()
    {
        var token = TokensFactory.Any();
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var userId = Guid.NewGuid();

        var requestUri = fixture.Route($"api/v1/users/{userId}");

        // Act
        var response = await client.DeleteAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.User.Delete.UserNotFound);
    }

    [Fact]
    public async Task Given_Delete_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        //Arrange
        var client = new HttpClient();

        var userUri = fixture.Route("api/v1/users");
        var users = await client.GetFromJsonAsync<List<UserReadModel>>(userUri);
        var user = users.First();

        var requestUri = fixture.Route($"api/v1/users/{user.Id}");

        var token = TokensFactory.WithId(user.Id);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await client.DeleteAsync(requestUri);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var result = await response.DeserializeResponseBody<ApiResult>();
        result.Should().BeNull();
    }
}