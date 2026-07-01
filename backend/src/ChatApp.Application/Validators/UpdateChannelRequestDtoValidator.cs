using ChatApp.Contracts.Channels.Requests;
using FluentValidation;

namespace ChatApp.Application.Validators;

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