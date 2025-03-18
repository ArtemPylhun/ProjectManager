using FluentValidation;

namespace Application.Users.Commands;

public class ResendEmailVerificationCommandValidator: AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}