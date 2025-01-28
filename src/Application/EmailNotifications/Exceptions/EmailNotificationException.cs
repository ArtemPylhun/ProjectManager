using Domain.Models.EmailNotifications;

namespace Application.EmailNotifications.Exceptions;

public class EmailNotificationException(EmailNotificationId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public EmailNotificationId Guid { get; } = id;
}

public class EmailNotificationNotFoundException(EmailNotificationId id)
    : EmailNotificationException(id, $"Email Notification under id: {id} not found!");

public class EmailNotificationAlreadyExistsException(EmailNotificationId id)
    : EmailNotificationException(id, $"Email Notification already exists: {id}!");

public class EmailNotificationUnknownException(EmailNotificationId id, Exception innerException)
    : EmailNotificationException(id, $"Unknown exception for the Email Notification under id: {id}!", innerException);