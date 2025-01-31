using Domain.Models.ProjectUsers;
using FluentValidation;

namespace Application.Projects.Commands;

public class AddUserToProjectCommandValidator: AbstractValidator<AddUserToProjectCommand>
{
    public AddUserToProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}