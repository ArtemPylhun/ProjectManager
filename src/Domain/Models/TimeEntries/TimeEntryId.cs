namespace Domain.Models.TimeEntries;

public record TimeEntryId(Guid Value)
{
    public static TimeEntryId New() => new(Guid.NewGuid());
    public static TimeEntryId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}