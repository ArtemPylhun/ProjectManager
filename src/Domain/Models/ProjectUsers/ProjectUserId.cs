namespace Domain.Models.ProjectUsers;
public record ProjectUserId(Guid Value)
{
    public static ProjectUserId New() => new(Guid.NewGuid());
    public static ProjectUserId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}