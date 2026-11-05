using ChatApp.Contracts.Channels.Requests;
using FluentValidation;

namespace ChatApp.Application.Validators;

/// <summary>
/// Validates channel update requests
/// </summary>
public class UpdateChannelRequestDtoValidator
    : AbstractValidator<UpdateChannelRequestDto>
{
    public UpdateChannelRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}