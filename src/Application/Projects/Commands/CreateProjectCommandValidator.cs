using Domain.Models.Projects;
using FluentValidation;

namespace Application.Projects.Commands;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CreatorId).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty(); //TODO maybe not needed
        RuleFor(x => x.ColorHex).NotEmpty().Must(x => x.StartsWith("#"))
            .WithMessage("Color must be written in a form of #RRGGBB").Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }
}