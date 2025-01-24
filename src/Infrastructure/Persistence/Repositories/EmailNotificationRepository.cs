using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Models.EmailNotifications;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class EmailNotificationRepository(ApplicationDbContext context): IEmailNotificationRepository, IEmailNotificationQueries
{
    public async Task<IReadOnlyList<EmailNotification>> GetAll(CancellationToken cancellationToken)
    {
        return await context.EmailNotifications
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<EmailNotification>> SearchByUser(Guid userId, CancellationToken cancellationToken)
    {
        var entity = await context.EmailNotifications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        return entity == null ? Option.None<EmailNotification>() : Option.Some(entity);
    }

    public async Task<Option<EmailNotification>> GetById(EmailNotificationId id, CancellationToken cancellationToken)
    {
        var entity = await context.EmailNotifications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<EmailNotification>() : Option.Some(entity);
    }
    public async Task<EmailNotification> Add(EmailNotification emailNotification, CancellationToken cancellationToken)
    {
         await context.EmailNotifications.AddAsync(emailNotification, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return emailNotification;
    }

    public async Task<EmailNotification> Update(EmailNotification emailNotification, CancellationToken cancellationToken)
    {
         context.EmailNotifications.Update(emailNotification);

        await context.SaveChangesAsync(cancellationToken);

        return emailNotification;
    }

    public async Task<EmailNotification> Delete(EmailNotification emailNotification, CancellationToken cancellationToken)
    {
        context.EmailNotifications.Remove(emailNotification);

        await context.SaveChangesAsync(cancellationToken);

        return emailNotification;
    }
}