using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserCommandValidator: AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(255).MinimumLength(3).EmailAddress();
    }
}