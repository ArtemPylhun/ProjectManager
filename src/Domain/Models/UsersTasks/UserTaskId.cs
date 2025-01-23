namespace Domain.Models.UsersTasks;

public record UserTaskId(Guid Value)
{
    public static UserTaskId New() => new(Guid.NewGuid());
    public static UserTaskId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}