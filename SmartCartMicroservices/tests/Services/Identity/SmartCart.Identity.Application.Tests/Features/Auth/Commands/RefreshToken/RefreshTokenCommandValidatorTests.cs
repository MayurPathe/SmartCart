using FluentValidation.TestHelper;
using SmartCart.Identity.Application.Features.Auth.Commands.RefreshToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Application.Tests.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator;

    public RefreshTokenCommandValidatorTests()
    {
        _validator = new RefreshTokenCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_RefreshToken_IsEmpty()
    {
        // Arrange
        var command = new RefreshTokenCommand("");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(
            x => x.RefreshToken);
    }

    [Fact]
    public void Should_Have_Error_When_RefreshToken_IsNull()
    {
        // Arrange
        var command = new RefreshTokenCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(
            x => x.RefreshToken);
    }

    [Fact]
    public void Should_Not_Have_Error_When_RefreshToken_IsValid()
    {
        // Arrange
        var command =
            new RefreshTokenCommand("valid-refresh-token");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
