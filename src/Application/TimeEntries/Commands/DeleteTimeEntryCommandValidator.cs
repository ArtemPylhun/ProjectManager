using FluentValidation;

namespace Application.TimeEntries.Commands;

public class DeleteTimeEntryCommandValidator: AbstractValidator<DeleteTimeEntryCommand>
{
    public DeleteTimeEntryCommandValidator()
    {
        
    }
}