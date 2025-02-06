using FluentValidation;

namespace Application.ProjectTasks.Commands;

public class RemoveUserFromProjectTaskCommandValidator: AbstractValidator<RemoveUserFromProjectTaskCommand>
{
    public RemoveUserFromProjectTaskCommandValidator()
    {
        RuleFor(x => x.UserTaskId).NotEmpty();
    }
}