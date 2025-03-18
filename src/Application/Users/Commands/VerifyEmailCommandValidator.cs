using FluentValidation;

namespace Application.Users.Commands;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}