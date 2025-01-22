namespace Domain.Models.Projects;

public record TaskId(Guid Value)
{
    public static TaskId New() => new(Guid.NewGuid());
    public static TaskId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}