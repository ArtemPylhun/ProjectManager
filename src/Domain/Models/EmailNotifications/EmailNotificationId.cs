namespace Domain.Models.EmailNotifications;

public record EmailNotificationId(Guid Value)
{
    public static EmailNotificationId New() => new(Guid.NewGuid());    
    public static EmailNotificationId  Empty() => new(Guid.Empty);
    public  override string ToString() => Value.ToString();
}