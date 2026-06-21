using ChatApp.Contracts.Messages.Requests;
using FluentValidation;

namespace ChatApp.Application.Validators;

public class MessageQueryDtoValidator
    : AbstractValidator<MessageQueryDto>
{
    public MessageQueryDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}