using System.Data;
using FluentValidation;

namespace Application.TimeEntries.Commands;

public class DeleteTimeEntryCommandValidator: AbstractValidator<DeleteTimeEntryCommand>
{
    public DeleteTimeEntryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}