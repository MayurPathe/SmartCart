using FluentValidation;

namespace SmartCart.Identity.Application.Features.Auth.Commands.Register;

public class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage(
                "Password must contain at least one uppercase character.")
            .Matches("[a-z]")
            .WithMessage(
                "Password must contain at least one lowercase character.")
            .Matches("[0-9]")
            .WithMessage(
                "Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage(
                "Password must contain at least one special character.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[0-9]{10}$")
            .WithMessage(
                "Phone number must contain exactly 10 digits.");
    }
}
