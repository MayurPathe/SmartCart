using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using SmartCart.Identity.Application.CommandHandlers;
using SmartCart.Identity.Application.Commands;
using SmartCart.Identity.Application.Tests.Fakes;
using SmartCart.Identity.Application.Validators;
using Xunit;
using SmartCart.Identity.Application;

namespace SmartCart.Identity.Application.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Register_User_When_Request_Is_Valid()
    {
        var repository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var jwtTokenService = new FakeJwtTokenService();
        var validator = new RegisterUserValidator();

        var handler = new RegisterUserCommandHandler(
            repository,
            passwordHasher,
            jwtTokenService,
            validator);

        var command = new RegisterUserCommand
        {
            FullName = "Yogesh Deshmukh",
            Email = "yogesh@test.com",
            Password = "Test@123",
            PhoneNumber = "9999999999"
        };

        var result = await handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.UserId.Should().NotBeEmpty();
        result.Email.Should().Be("yogesh@test.com");
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_Error_When_Email_Is_Invalid()
    {
        var repository = new FakeUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var jwtTokenService = new FakeJwtTokenService();
        var validator = new RegisterUserValidator();

        var handler = new RegisterUserCommandHandler(
            repository,
            passwordHasher,
            jwtTokenService,
            validator);

        var command = new RegisterUserCommand
        {
            FullName = "Yogesh Deshmukh",
            Email = "invalid-email",
            Password = "Test@123"
        };

        var action = async () => await handler.HandleAsync(command);

        await action.Should().ThrowAsync<ArgumentException>();
    }
}
