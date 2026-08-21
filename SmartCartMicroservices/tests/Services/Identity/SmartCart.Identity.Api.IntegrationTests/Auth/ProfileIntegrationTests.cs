using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Api.IntegrationTests.Auth;

public class ProfileIntegrationTests
    : IntegrationTestBase
{
    public ProfileIntegrationTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }


    [Fact]
    public async Task Profile_Should_ReturnUserProfile_When_UserIsAuthenticated()
    {
        // Arrange

        var email =
            $"profile-{Guid.NewGuid()}@gmail.com";

        var password =
            "Password123!";

        var registerResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                new
                {
                    FullName = "Profile Test User",
                    Email = email,
                    Password = password,
                    PhoneNumber = "9876543210"
                });

        registerResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

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

        var response =
            await Client.GetAsync(
                "/api/auth/profile");

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var profile =
            await response.Content
                .ReadFromJsonAsync<UserProfileDto>();

        profile.Should()
            .NotBeNull();

        profile!.Email
            .Should()
            .Be(email);

        profile.FullName
            .Should()
            .Be("Profile Test User");

        profile.PhoneNumber
            .Should()
            .Be("9876543210");
    }


    [Fact]
    public async Task Profile_Should_ReturnUnauthorized_When_UserIsNotAuthenticated()
    {
        // Arrange

        Client.DefaultRequestHeaders.Authorization = null;

        // Act

        var response =
            await Client.GetAsync(
                "/api/auth/profile");

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task Profile_Should_ReturnUnauthorized_When_AccessTokenIsInvalid()
    {
        // Arrange

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "invalid-access-token");

        // Act

        var response =
            await Client.GetAsync(
                "/api/auth/profile");

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }
}