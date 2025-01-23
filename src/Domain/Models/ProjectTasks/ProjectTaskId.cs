namespace Domain.Models.ProjectTasks;

public record ProjectTaskId(Guid Value)
{
    public static ProjectTaskId New() => new(Guid.NewGuid());
    public static ProjectTaskId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}