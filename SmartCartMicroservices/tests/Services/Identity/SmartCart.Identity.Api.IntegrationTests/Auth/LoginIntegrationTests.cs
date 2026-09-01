using System.Net;
using System.Net.Http.Json;

using FluentAssertions;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Api.IntegrationTests.Auth;

public class LoginIntegrationTests
    : IntegrationTestBase
{
    public LoginIntegrationTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }


    [Fact]
    public async Task Login_Should_ReturnTokens_When_CredentialsAreValid()
    {
        // Arrange

        var email =
            $"login-{Guid.NewGuid()}@gmail.com";

        var password =
            "Password123!";

        var registerRequest = new
        {
            FullName = "Login Test User",
            Email = email,
            Password = password,
            PhoneNumber = "9876543210"
        };

        var registerResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                registerRequest);

        registerResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>();

        result.Should()
            .NotBeNull();

        result!.AccessToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RefreshToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.ExpiresAt
            .Should()
            .BeAfter(DateTime.UtcNow);
    }


    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_PasswordIsIncorrect()
    {
        // Arrange

        var email =
            $"wrong-password-{Guid.NewGuid()}@gmail.com";

        await Client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                FullName = "Test User",
                Email = email,
                Password = "Password123!",
                PhoneNumber = "9876543210"
            });

        var loginRequest = new
        {
            Email = email,
            Password = "WrongPassword123!"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_UserDoesNotExist()
    {
        // Arrange

        var request = new
        {
            Email = $"not-exist-{Guid.NewGuid()}@gmail.com",
            Password = "Password123!"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Login_Should_ReturnBadRequest_When_EmailIsInvalid()
    {
        // Arrange

        var request = new
        {
            Email = "invalid-email",
            Password = "Password123!"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }
}