using FluentValidation;

namespace Application.Users.Commands;

public class LoginUserCommandValidator: AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.EmailOrUsername).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}