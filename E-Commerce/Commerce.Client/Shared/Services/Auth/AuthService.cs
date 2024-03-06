using System.Net.Http.Json;
using Blazored.LocalStorage;
using CSharpFunctionalExtensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;

namespace Commerce.Client;

public interface IAuthService
{
    Task RefreshToken();

    Task<Result<IdentifiedUser>> GetIdentifiedUser();

    Task<Maybe<string>> GetToken();

    Task DeleteToken(bool forceMove = true);
}

internal sealed class AuthService(ILocalStorageService localStorageService, NavigationManager navigationManager, HttpClient http) : IAuthService
{
    public async Task RefreshToken()
    {
        var tokenResult = await GetToken().ToResult("Token not found");
        if (tokenResult.IsFailure)
        {
            await DeleteToken(false);
            return;
        }

        var refreshTokenResult = await GetRefreshToken().ToResult("Refresh token not found");
        if (refreshTokenResult.IsFailure)
        {
            await DeleteToken(false);
            return;
        }

        var refreshTokenModel = new RefreshTokenModel
        {
            ExpiredToken = tokenResult.Value,
            RefreshToken = refreshTokenResult.Value
        };

        var response = await http.PostAsJsonAsync(Routes.Auth.RefreshToken, refreshTokenModel);
        if (!response.IsSuccessStatusCode)
        {
            await DeleteToken();
            return;
        }

        var refreshTokenResponse = await response.Content.ReadFromJsonAsync<ApiResult<RefreshTokenResponse>>();
        if (refreshTokenResponse.IsFailure)
        {
            await DeleteToken();
            return;
        }

        await localStorageService.SetItemAsync("authToken", refreshTokenResponse.Value.AuthToken);
        await localStorageService.SetItemAsync("refreshToken", refreshTokenResponse.Value.RefreshToken.Value);
    }

    public async Task<Result<IdentifiedUser>> GetIdentifiedUser()
    {
        var tokenResult = await GetToken().ToResult("Token not found");
        if (tokenResult.IsFailure)
        {
            return Result.Failure<IdentifiedUser>(tokenResult.Error);
        }

        var token = tokenResult.Value;
        var jwtHandler = new JwtSecurityTokenHandler();

        var jwtSettings = new JwtConfiguration();
        var validationParams = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtSettings.SigningKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(10)
        };

        try
        {
            jwtHandler.ValidateToken(token, validationParams, out _);
            var identifiedUser = IdentifiedUser.From(jwtHandler.ReadJwtToken(token));
            return Result.Success(identifiedUser);
        }
        catch (Exception)
        {
            return Result.Failure<IdentifiedUser>("Invalid token");
        }
    }

    public async Task<Maybe<string>> GetToken()
    {
        return Maybe<string>
            .From(await localStorageService.GetItemAsStringAsync("authToken"))
            .Map(t => t[1..^1]);
    }

    public async Task<Maybe<string>> GetRefreshToken()
    {
        return Maybe<string>
            .From(await localStorageService.GetItemAsStringAsync("refreshToken"))
            .Map(t => t[1..^1]);
    }

    public async Task DeleteToken(bool forceMove = true)
    {
        await localStorageService.RemoveItemAsync("authToken");
        await localStorageService.RemoveItemAsync("refreshToken");

        if (forceMove)
        {
            var destination = "/categories";
            if (navigationManager.Uri.Contains("categories"))
            {
                navigationManager.Refresh(true);
            }
            else
            {
                navigationManager.NavigateTo(destination, true);
            }
        }
    }

    private sealed class RefreshTokenModel
    {
        public string ExpiredToken { get; set; }

        public string RefreshToken { get; set; }
    }

    private sealed class RefreshTokenResponse
    {
        public string AuthToken { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }
}

public sealed class CommerceClaims
{
    public const string UserId = "userId";
    public const string UserEmail = "userEmail";
    public const string UserFirstName = "userFirstName";
    public const string UserLastName = "userLastName";
}
