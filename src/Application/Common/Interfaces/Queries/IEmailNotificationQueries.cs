using Domain.Models.EmailNotifications;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IEmailNotificationQueries
{
    Task<IReadOnlyList<EmailNotification>> GetAll(CancellationToken cancellationToken);
    Task<Option<EmailNotification>> SearchByUser(Guid userId, CancellationToken cancellationToken);
    Task<Option<EmailNotification>> GetById(EmailNotificationId id, CancellationToken cancellationToken);
}