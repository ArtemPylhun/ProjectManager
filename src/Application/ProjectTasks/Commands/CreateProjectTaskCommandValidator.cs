using FluentValidation;

namespace Application.ProjectTasks.Commands;

public class CreateProjectTaskCommandValidator: AbstractValidator<CreateProjectTaskCommand>
{
    public CreateProjectTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.CreatorId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(5).MaximumLength(100);
        RuleFor(x => x.EstimatedTime).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MinimumLength(20).MaximumLength(1000);
    }
}