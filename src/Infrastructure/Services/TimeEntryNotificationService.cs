using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Domain.Models.Users;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TimeEntryNotificationService(
    ITimeEntryQueries timeEntryQueries,
    UserManager<User> userManager,
    IEmailService emailService) : ITimeEntryNotificationService
{
    private const int RequiredDailyMinutes = 480;
    public async Task ScheduleTimeEntryNotifications()
    {
        // Get all users (assuming you have a way to fetch all users, e.g., via IUserQueries)
        var users = await userManager.Users.ToListAsync(); // You'll need to implement or inject this

        foreach (var user in users)
        {
            // Check time entries for yesterday for this user using the new query
            var yesterday = DateTime.UtcNow.Date.AddDays(-1); // Check yesterday instead of today
            var yesterdaysEntries = await timeEntryQueries.GetDailyTimeEntriesForUser(user.Id, yesterday, CancellationToken.None);

            // Check conditions for notification
            bool noEntries = !yesterdaysEntries.Any();
            bool insufficientMinutes = yesterdaysEntries.Sum(e => e.Minutes) < RequiredDailyMinutes;
            int currentMinutes = yesterdaysEntries.Sum(e => e.Minutes);

            if (noEntries || insufficientMinutes)
            {
                var notificationTime = DateTime.UtcNow.Date.AddHours(7);
                if (notificationTime < DateTime.UtcNow)
                {
                    notificationTime = notificationTime.AddDays(1);
                }
                BackgroundJob.Schedule(
                    () => SendTimeEntryReminder(user.Email!, noEntries, insufficientMinutes, currentMinutes),
                    notificationTime - DateTime.UtcNow);
            }
        }
    }

    public void SendTimeEntryReminder(string email, bool noEntries, bool insufficientMinutes, int currentMinutes)
    {
        var subject = "Time Entry Reminder";
        var message = "Hello,\n\n";

        if (noEntries)
        {
            message += "You have not reported any time entries for yesterday. Please log your time to ensure accurate tracking.\n\n";
        }
        if (insufficientMinutes)
        {
            message += $"You have only reported {currentMinutes} minutes yesterday, which is less than the required 480 minutes (8 hours). Please update your time entries.\n\n";
        }

        message += "Best regards,\nYour Time Tracking Team";

        emailService.SendEmail(email, subject, message);
    }
}