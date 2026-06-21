using ChatApp.Contracts.Messages.Requests;
using FluentValidation;

namespace ChatApp.Application.Validators;

public class CreateMessageRequestDtoValidator
    : AbstractValidator<CreateMessageRequestDto>
{
    public CreateMessageRequestDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Content.Trim())
            .NotEmpty();
    }
}