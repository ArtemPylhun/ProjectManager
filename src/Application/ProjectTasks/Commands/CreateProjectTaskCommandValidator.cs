using FluentValidation;

namespace Application.ProjectTasks.Commands;

public class CreateProjectTaskCommandValidator: AbstractValidator<CreateProjectTaskCommand>
{
    public CreateProjectTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EstimatedTime).NotEmpty();
    }
}