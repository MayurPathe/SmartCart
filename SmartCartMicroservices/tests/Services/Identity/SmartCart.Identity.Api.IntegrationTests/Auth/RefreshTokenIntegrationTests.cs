using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Api.IntegrationTests.Auth;

public class RefreshTokenIntegrationTests
    : IntegrationTestBase
{
    public RefreshTokenIntegrationTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }


    [Fact]
    public async Task RefreshToken_Should_ReturnNewTokens_When_TokenIsValid()
    {
        // Arrange

        var email =
            $"refresh-{Guid.NewGuid()}@gmail.com";

        var password =
            "Password123!";

        await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                FullName = "Refresh Test User",
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

        loginResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        loginResult.Should()
            .NotBeNull();

        var oldRefreshToken =
            loginResult!.RefreshToken;

        // Act

        var refreshResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/refresh-token",
                new
                {
                    RefreshToken = oldRefreshToken
                });

        // Assert

        refreshResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var result =
            await refreshResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        result.Should()
            .NotBeNull();

        result!.AccessToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RefreshToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RefreshToken
            .Should()
            .NotBe(oldRefreshToken);
    }


    [Fact]
    public async Task RefreshToken_Should_ReturnUnauthorized_When_TokenIsInvalid()
    {
        // Arrange

        var request = new
        {
            RefreshToken =
                "invalid-refresh-token"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/refresh-token",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task RefreshToken_Should_ReturnBadRequest_When_TokenIsEmpty()
    {
        // Arrange

        var request = new
        {
            RefreshToken = ""
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/refresh-token",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task RefreshToken_Should_NotAllowOldTokenAfterRotation()
    {
        // Arrange

        var email =
            $"rotation-{Guid.NewGuid()}@gmail.com";

        var password =
            "Password123!";

        await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                FullName = "Rotation Test",
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

        var oldRefreshToken =
            loginResult!.RefreshToken;

        // First refresh
        var firstRefresh =
            await Client.PostAsJsonAsync(
                "/api/auth/refresh-token",
                new
                {
                    RefreshToken =
                        oldRefreshToken
                });

        firstRefresh.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        // Act - use old token again

        var secondRefresh =
            await Client.PostAsJsonAsync(
                "/api/auth/refresh-token",
                new
                {
                    RefreshToken =
                        oldRefreshToken
                });

        // Assert

        secondRefresh.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }
}