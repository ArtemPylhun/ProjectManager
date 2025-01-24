using Domain.Models.EmailNotifications;

namespace Application.Common.Interfaces.Repositories;

public interface IEmailNotificationRepository
{
    Task<EmailNotification> Add(EmailNotification emailNotification, CancellationToken cancellationToken);
    Task<EmailNotification> Update(EmailNotification emailNotification, CancellationToken cancellationToken);
    Task<EmailNotification> Delete(EmailNotification emailNotification, CancellationToken cancellationToken);
}