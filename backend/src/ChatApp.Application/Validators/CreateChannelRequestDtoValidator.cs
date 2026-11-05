using ChatApp.Contracts.Channels.Requests;
using FluentValidation;

namespace ChatApp.Application.Validators;

/// <summary>
/// Validates channel creation requests
/// </summary>
public class CreateChannelRequestDtoValidator
    : AbstractValidator<CreateChannelRequestDto>
{
    public CreateChannelRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}