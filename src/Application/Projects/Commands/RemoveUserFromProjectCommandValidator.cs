using FluentValidation;

namespace Application.Projects.Commands;

public class RemoveUserFromProjectCommandValidator: AbstractValidator<RemoveUserFromProjectCommand>
{
    public RemoveUserFromProjectCommandValidator()
    {
        RuleFor(x => x.ProjectUserId).NotEmpty();
    }
}