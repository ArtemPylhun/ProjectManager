using FluentValidation;

namespace Application.Roles.Commands;

public class UpdateRoleCommandValidator: AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(100);
    }
}