using FluentValidation;

namespace Application.ProjectTasks.Commands;

public class DeleteProjectTaskCommandValidator: AbstractValidator<DeleteProjectTaskCommand>
{
    public DeleteProjectTaskCommandValidator()
    {
        RuleFor(x => x.ProjectTaskId).NotEmpty();
    }
}