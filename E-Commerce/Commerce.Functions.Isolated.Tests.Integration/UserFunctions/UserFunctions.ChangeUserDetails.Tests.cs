using Xunit;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Commerce.Domain;
using Newtonsoft.Json;
using FluentAssertions;
using Commerce.Business;

namespace Commerce.Functions.Isolated.Tests.Integration;

public sealed class UserFunctionsChangeUserDetailsTests(AzureFunctionsTestContainersFixture fixture) : IClassFixture<AzureFunctionsTestContainersFixture>
{
    [Fact]
    public async Task Given_ChangeUserDetails_When_UserNotFound_Then_ShouldFail()
    {
        // Arrange
        var requestUri = fixture.Route($"api/v1/users/{Guid.NewGuid()}/details");

        var command = new ChangeUserDetailsCommand(Guid.NewGuid(), "John", "Doe");
        var reqAsJson = JsonConvert.SerializeObject(command);
        
        var token = TokensFactory.Any();
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await http.PutAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(BusinessErrors.User.ChangeDetails.UserNotFound);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public async Task Given_ChangeUserDetails_When_FirstNameNullOrEmpty_Then_ShouldFail(string badFirstName)
    {
        // Arrange
        var users = await new HttpClient().GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"));
        var user = users.First();

        var command = new ChangeUserDetailsCommand(user.Id, badFirstName, "new last name");
        var requestUri = fixture.Route($"api/v1/users/{user.Id}/details");
        var reqAsJson = JsonConvert.SerializeObject(command);

        var token = user.GetToken();
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await http.PutAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.ChangeDetails.FirstNameNullOrEmpty);
    }

    [Fact]
    public async Task Given_Create_When_FirstNameLongerThanMaxLength_Then_ShouldFail()
    {
        // Arrange
        var users = await new HttpClient().GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"));
        var user = users.First();

        var command = new ChangeUserDetailsCommand(user.Id, string.Join("", Enumerable.Repeat("a", DomainConstants.User.LastNameMaxLength + 1)), "new last name");
        var requestUri = fixture.Route($"api/v1/users/{user.Id}/details");
        var reqAsJson = JsonConvert.SerializeObject(command);

        var token = user.GetToken();
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await http.PutAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.ChangeDetails.FirstNameLongerThanMaxLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public async Task Given_Create_When_LastNameNullOrEmpty_Then_ShouldFail(string badLastName)
    {
        // Arrange
        var users = await new HttpClient().GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"));
        var user = users.First();

        var command = new ChangeUserDetailsCommand(user.Id, "john", badLastName);
        var requestUri = fixture.Route($"api/v1/users/{user.Id}/details");
        var reqAsJson = JsonConvert.SerializeObject(command);

        var token = user.GetToken();
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await http.PutAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.ChangeDetails.LastNameNullOrEmpty);
    }

    [Fact]
    public async Task Given_Create_When_LastNameLongerThanMaxLength_Then_ShouldFail()
    {
        // Arrange
        var users = await new HttpClient().GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"));
        var user = users.First();

        var command = new ChangeUserDetailsCommand(user.Id, "new name", string.Join("", Enumerable.Repeat("a", DomainConstants.User.LastNameMaxLength + 1)));
        var requestUri = fixture.Route($"api/v1/users/{user.Id}/details");
        var reqAsJson = JsonConvert.SerializeObject(command);

        var token = user.GetToken();
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await http.PutAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be(DomainErrors.User.ChangeDetails.LastNameLongerThanMaxLength);
    }

    [Fact]
    public async Task Given_Create_When_NotViolatingConstraints_Then_ShouldSucceed()
    {
        // Arrange
        var users = await new HttpClient().GetFromJsonAsync<IEnumerable<UserReadModel>>(fixture.Route("api/v1/users"));
        var user = users.First();

        var command = new ChangeUserDetailsCommand(user.Id, "new name", "new last name");
        var requestUri = fixture.Route($"api/v1/users/{user.Id}/details");
        var reqAsJson = JsonConvert.SerializeObject(command);

        var token = user.GetToken();
        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await http.PutAsync(requestUri, new StringContent(reqAsJson, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var result = await response.DeserializeResponseBody<ApiResult>();
        result.Should().BeNull();
    }
}