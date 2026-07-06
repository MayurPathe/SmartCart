using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SmartCart.Identity.Application.DTOs;
using Xunit;

namespace SmartCart.Identity.Api.IntegrationTests;

public class AuthControllerTests
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_Return_Ok_When_Request_Is_Valid()
    {
        var request = new RegisterRequestDto
        {
            FullName = "Yogesh Deshmukh",
            Email = $"yogesh_{Guid.NewGuid()}@test.com",
            Password = "Test@123",
            PhoneNumber = "9999999999"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Should_Return_Ok_When_User_Is_Registered()
    {
        var email = $"login_{Guid.NewGuid()}@test.com";

        var registerRequest = new RegisterRequestDto
        {
            FullName = "Login User",
            Email = email,
            Password = "Test@123",
            PhoneNumber = "9999999999"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginRequest = new LoginRequestDto
        {
            Email = email,
            Password = "Test@123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }
}
