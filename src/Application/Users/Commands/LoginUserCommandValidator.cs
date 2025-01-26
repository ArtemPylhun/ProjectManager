using FluentValidation;

namespace Application.Users.Commands;

public class LoginUserCommandValidator: AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) 
                                || !string.IsNullOrWhiteSpace(x.UserName))
            .WithMessage("Email or username must be provided");
        RuleFor(x => x.Password).NotEmpty();
    }
}