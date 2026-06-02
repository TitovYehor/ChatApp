using ChatApp.Application.DTOs.Workspaces;
using FluentValidation;

namespace ChatApp.Application.Validators;

public class CreateWorkspaceRequestDtoValidator
    : AbstractValidator<CreateWorkspaceRequestDto>
{
    public CreateWorkspaceRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}