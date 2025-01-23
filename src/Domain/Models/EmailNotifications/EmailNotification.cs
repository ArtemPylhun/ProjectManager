using Domain.Models.Users;

namespace Domain.Models.EmailNotifications;

public class EmailNotification
{
    public EmailNotificationId Id { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public NotificationType NotificationType { get; private set; }

    private EmailNotification(EmailNotificationId id, string subject, string body, Guid userId,
        NotificationType notificationType)
    {
        Id = id;
        Subject = subject;
        Body = body;
        UserId = userId;
        NotificationType = notificationType;
    }

    public static EmailNotification New(EmailNotificationId id, string subject, string body, Guid userId,
        NotificationType notificationType) => new(id, subject, body, userId, notificationType);
}

public enum NotificationType
{
    NotEnoughHours = 1,
    HoursNotTracked = 2
}