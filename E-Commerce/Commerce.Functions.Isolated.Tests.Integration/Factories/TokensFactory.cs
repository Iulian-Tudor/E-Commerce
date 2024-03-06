using System.Text;
using Commerce.Business;
using System.Security.Claims;
using Commerce.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Commerce.Functions.Isolated.Tests.Integration;

public static class TokensFactory
{
    public static string Any()
    {
        var claims = new List<Claim>
        {
            new(CommerceClaims.UserId, Guid.NewGuid().ToString()),
            new(CommerceClaims.UserEmail, "randommail@me.com"),
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

        return tokenHandler.WriteToken(jwtAuthToken);
    }

    public static string WithId(Guid id)
    {
        var claims = new List<Claim>
        {
            new(CommerceClaims.UserId, id.ToString()),
            new(CommerceClaims.UserEmail, "randommail@me.com"),
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

        return tokenHandler.WriteToken(jwtAuthToken);
    }

    public static string GetToken(this UserReadModel user)
    {
        var claims = new List<Claim>
        {
            new(CommerceClaims.UserId, user.Id.ToString()),
            new(CommerceClaims.UserEmail, user.EmailAddress),
            new(CommerceClaims.UserFirstName, user.FirstName),
            new(CommerceClaims.UserLastName, user.LastName),
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

        return tokenHandler.WriteToken(jwtAuthToken);
    }
}