using FluentValidation;

namespace Application.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(255).MinimumLength(3).EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
}