using System.Text;
using Newtonsoft.Json;
using Commerce.Business;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Net.Http.Headers;
using Commerce.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Commerce.Functions.Isolated.Tests.Integration;

public static class Seed
{
    public static async Task SeedDatabase(this AzureFunctionsTestContainersFixture fixture)
    {
        var claims = new List<Claim>
        {
            new(CommerceClaims.UserId, Guid.NewGuid().ToString()),
            new(CommerceClaims.UserEmail, "john.doe@mail.com"),
            new(CommerceClaims.UserFirstName, "John"),
            new(CommerceClaims.UserLastName, "Doe"),
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("helloThisIsAKeyWhichIsSuperDuperSecure")), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "Commerce",
            Audience = "Commerce",
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtAuthToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(jwtAuthToken);

        var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var userRequests = new List<CreateUserCommand>
        {
            new("John", "Doe", "john@mail.com"),
            new("Jane", "Doe", "jane@mail.ro"),
            new("John", "Smith", "jsmith@email.fr")
        };

        var usersUri = new UriBuilder(
            Uri.UriSchemeHttp,
            fixture.AzureFunctionsContainerInstance.Hostname,
            fixture.AzureFunctionsContainerInstance.GetMappedPublicPort(80),
            "api/v1/users"
        );
        foreach (var request in userRequests)
        {
            var reqAsJson = JsonConvert.SerializeObject(request);
            var res = await http.PostAsync(usersUri.ToString(), new StringContent(reqAsJson, Encoding.UTF8, "application/json"));
            var apiResult = await res.DeserializeResponseBody<ApiResult>();
        }
        
        var allUsers = await http.GetFromJsonAsync<List<UserReadModel>>(usersUri.ToString());
        var users = allUsers.Select(u => u.Id).ToList();

        var categoryRequests = new List<CreateCategoryCommand>
        {
            new("Bread", "This is a category for different kinds of bread"),
            new("Dairy", "This is a category for different kinds of dairy products"),
            new("Fruits", "This is a category for different kinds of fruits"),
        };

        var categoriesUri = new UriBuilder(
            Uri.UriSchemeHttp,
            fixture.AzureFunctionsContainerInstance.Hostname,
            fixture.AzureFunctionsContainerInstance.GetMappedPublicPort(80),
            "api/v1/categories"
        );
        foreach (var request in categoryRequests)
        {
            var reqAsJson = JsonConvert.SerializeObject(request);
            var res = await http.PostAsync(categoriesUri.ToString(), new StringContent(reqAsJson, Encoding.UTF8, "application/json"));
        }
        var categories = await http.GetFromJsonAsync<List<CategoryReadModel>>(categoriesUri.ToString());

        var productsUri = new UriBuilder(
            Uri.UriSchemeHttp,
            fixture.AzureFunctionsContainerInstance.Hostname,
            fixture.AzureFunctionsContainerInstance.GetMappedPublicPort(80),
            "api/v1/products"
        );
        var productRequests = new List<CreateProductCommand>
        {
            new(users[0], "Franzela", "This is a franzela", 1.5m, categories[0].Id),
            new(users[0], "Paine", "This is a paine", 2.5m, categories[0].Id),
            new(users[0], "Lapte", "This is a lapte", 3.5m, categories[1].Id),
            new(users[1], "Iaurt", "This is a iaurt", 4.5m, categories[1].Id),
            new(users[1], "Banana", "This is a banana", 5.5m, categories[2].Id),
            new(users[1], "Mar", "This is a mar", 6.5m, categories[2].Id),
            new(users[2], "Portocala", "This is a portocala", 7.5m, categories[2].Id),
            new(users[2], "Mere", "This is a mere", 8.5m, categories[2].Id),
            new(users[2], "Piersici", "This is a piersica", 9.5m, categories[2].Id),
        };

        foreach (var request in productRequests)
        {
            var reqAsJson = JsonConvert.SerializeObject(request);
            await http.PostAsync(productsUri.ToString(), new StringContent(reqAsJson, Encoding.UTF8, "application/json"));
        }
    }
}