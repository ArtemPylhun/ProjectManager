using FluentValidation;

namespace Application.ProjectTasks.Commands;

public class AddUserToProjectTaskCommandValidator: AbstractValidator<AddUserToProjectTaskCommand>
{
    public AddUserToProjectTaskCommandValidator()
    {
        RuleFor(x => x.ProjectTaskId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}