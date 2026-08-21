using FluentValidation.TestHelper;
using SmartCart.Identity.Application.Features.Auth.Commands.Register;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Commands.Register;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Should_Reject_WeakPassword()
    {
        var command = new RegisterUserCommand(
            "Mayur",
            "test@gmail.com",
            "password",
            "9876543210");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345678")]
    [InlineData("password123!")]
    [InlineData("PASSWORD123!")]
    [InlineData("Password")]
    public void Should_Reject_Invalid_Password(string password)
    {
        var command = new RegisterUserCommand(
            "Mayur",
            "test@gmail.com",
            password,
            "9876543210");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Accept_ValidPassword()
    {
        var command = new RegisterUserCommand(
            "Mayur",
            "test@gmail.com",
            "Password123!",
            "9876543210");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
