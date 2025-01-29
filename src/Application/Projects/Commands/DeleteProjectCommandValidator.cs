using FluentValidation;

namespace Application.Projects.Commands;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}