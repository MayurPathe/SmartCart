using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using SmartCart.Identity.Application.Commands;
using SmartCart.Identity.Application.Validators;
using Xunit;

namespace SmartCart.Identity.Application.Tests.Validators;

public class LoginUserValidatorTests
{
    [Fact]
    public void Validate_Should_Return_Error_When_Email_Is_Empty()
    {
        var validator = new LoginUserValidator();

        var command = new LoginUserCommand
        {
            Email = "",
            Password = "Test@123"
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Should_Be_Valid_When_Request_Is_Correct()
    {
        var validator = new LoginUserValidator();

        var command = new LoginUserCommand
        {
            Email = "test@test.com",
            Password = "Test@123"
        };

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
