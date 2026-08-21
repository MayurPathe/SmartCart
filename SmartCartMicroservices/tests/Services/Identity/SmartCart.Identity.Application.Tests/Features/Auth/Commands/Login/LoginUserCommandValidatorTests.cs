using FluentValidation.TestHelper;
using SmartCart.Identity.Application.Features.Auth.Commands.Login;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Commands.Login;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_IsEmpty()
    {
        var command =
            new LoginUserCommand(
                "",
                "Password123!",
                null,
                null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_IsInvalid()
    {
        var command =
            new LoginUserCommand(
                "invalid-email",
                "Password123!",
                null,
                null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_IsEmpty()
    {
        var command =
            new LoginUserCommand(
                "test@gmail.com",
                "",
                null,
                null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_IsValid()
    {
        var command =
            new LoginUserCommand(
                "test@gmail.com",
                "Password123!",
                null,
                null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}