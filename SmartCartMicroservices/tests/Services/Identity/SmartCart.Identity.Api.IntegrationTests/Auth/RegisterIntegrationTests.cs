using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SmartCart.Identity.Application.DTOs;

namespace SmartCart.Identity.Api.IntegrationTests.Auth;

public class RegisterIntegrationTests
    : IntegrationTestBase
{
    public RegisterIntegrationTests(
        CustomWebApplicationFactory factory)
        : base(factory)
    {
    }


    [Fact]
    public async Task Register_Should_ReturnOk_When_RequestIsValid()
    {
        // Arrange

        var request = new
        {
            FullName = "Integration Test User",
            Email = $"test-{Guid.NewGuid()}@gmail.com",
            Password = "Password123!",
            PhoneNumber = "9876543210"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<RegisterResponse>();

        result.Should()
            .NotBeNull();

        result!.UserId
            .Should()
            .NotBe(Guid.Empty);

        result.FullName
            .Should()
            .Be(request.FullName);

        result.Email
            .Should()
            .Be(request.Email);

        result.Role
            .Should()
            .Be("User");
    }


    [Fact]
    public async Task Register_Should_ReturnBadRequest_When_EmailIsInvalid()
    {
        // Arrange

        var request = new
        {
            FullName = "Test User",
            Email = "invalid-email",
            Password = "Password123!",
            PhoneNumber = "9876543210"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Register_Should_ReturnBadRequest_When_PasswordIsWeak()
    {
        // Arrange

        var request = new
        {
            FullName = "Test User",
            Email = $"test-{Guid.NewGuid()}@gmail.com",
            Password = "123",
            PhoneNumber = "9876543210"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Register_Should_ReturnBadRequest_When_PhoneNumberIsInvalid()
    {
        // Arrange

        var request = new
        {
            FullName = "Test User",
            Email = $"test-{Guid.NewGuid()}@gmail.com",
            Password = "Password123!",
            PhoneNumber = "12345"
        };

        // Act

        var response =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task Register_Should_ReturnError_When_EmailAlreadyExists()
    {
        // Arrange

        var email =
            $"duplicate-{Guid.NewGuid()}@gmail.com";

        var request = new
        {
            FullName = "Test User",
            Email = email,
            Password = "Password123!",
            PhoneNumber = "9876543210"
        };

        // First registration
        var firstResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                request);

        firstResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        // Act - second registration

        var secondResponse =
            await Client.PostAsJsonAsync(
                "/api/auth/register",
                request);

        // Assert

        secondResponse.StatusCode
            .Should()
            .NotBe(HttpStatusCode.OK);
    }
}