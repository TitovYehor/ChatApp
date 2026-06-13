using ChatApp.Application.DTOs.Messages;
using FluentValidation;

namespace ChatApp.Application.Validators;

public class UpdateMessageRequestDtoValidator
    : AbstractValidator<UpdateMessageRequestDto>
{
    public UpdateMessageRequestDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}