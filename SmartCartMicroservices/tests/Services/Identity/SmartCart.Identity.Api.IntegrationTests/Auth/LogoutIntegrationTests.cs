using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Api.IntegrationTests.Auth;

public class LogoutIntegrationTests
    : IntegrationTestBase
{
    public LogoutIntegrationTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }


    [Fact]
    public async Task Logout_Should_ReturnOk_When_UserIsAuthenticated()
    {
        // Arrange

        var email =
            $"logout-{Guid.NewGuid()}@gmail.com";

        var password =
            "Password123!";

        await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                FullName = "Logout Test User",
                Email = email,
                Password = password,
                PhoneNumber = "9876543210"
            });

        var loginResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = email,
                    Password = password
                });

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        loginResult.Should()
            .NotBeNull();

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.AccessToken);

        // Act

        var logoutResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/logout",
                new
                {
                    RefreshToken =
                        loginResult.RefreshToken
                });

        // Assert

        logoutResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task Logout_Should_ReturnUnauthorized_When_UserIsNotAuthenticated()
    {
        // Arrange

        Client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            RefreshToken = "some-refresh-token"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/logout",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Logout_Should_RevokeRefreshToken()
    {
        // Arrange

        var email =
            $"logout-revoke-{Guid.NewGuid()}@gmail.com";

        var password =
            "Password123!";

        await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                FullName = "Logout Revoke Test",
                Email = email,
                Password = password,
                PhoneNumber = "9876543210"
            });

        var loginResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = email,
                    Password = password
                });

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.AccessToken);

        var refreshToken =
            loginResult.RefreshToken;

        // Logout

        var logoutResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/logout",
                new
                {
                    RefreshToken =
                        refreshToken
                });

        logoutResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        // Act - Try using the revoked token

        var refreshResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/refresh-token",
                new
                {
                    RefreshToken =
                        refreshToken
                });

        // Assert

        refreshResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }
}