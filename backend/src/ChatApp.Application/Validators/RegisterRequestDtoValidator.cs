using ChatApp.Application.DTOs.Auth;
using FluentValidation;

namespace ChatApp.Application.Validators;

public class RegisterRequestDtoValidator
    : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(30)
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage(
                "Username may contain only letters, numbers and underscore");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit");
    }
}