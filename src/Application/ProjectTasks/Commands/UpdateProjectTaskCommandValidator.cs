using Application.Projects.Commands;
using FluentValidation;

namespace Application.ProjectTasks.Commands;

public class UpdateProjectTaskCommandValidator: AbstractValidator<UpdateProjectTaskCommand>
{
    public UpdateProjectTaskCommandValidator()
    {
        RuleFor(x => x.ProjectTaskId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(5).MaximumLength(100);
        RuleFor(x => x.EstimatedTime).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MinimumLength(20).MaximumLength(1000);

    }
}