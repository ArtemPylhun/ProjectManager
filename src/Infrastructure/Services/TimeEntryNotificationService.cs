using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Domain.Models.Users;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public class TimeEntryNotificationService(
    ITimeEntryQueries timeEntryQueries,
    UserManager<User> userManager,
    IEmailService emailService,
    IEmailViewRenderer emailViewRenderer) : ITimeEntryNotificationService
{
    private const int RequiredDailyMinutes = 480;
    private const int PreviousDayNumber = -1;
    private const int HoursNumberForSendingEmail = 7;
    private const int NextDayNumber = 1;

    public async Task ScheduleTimeEntryNotifications()
    {
        // Get all users
        var users = await userManager.Users.ToListAsync();

        foreach (var user in users)
        {
            // Check time entries for yesterday for this user using the new query
            var yesterday = DateTime.UtcNow.Date.AddDays(PreviousDayNumber);
            var yesterdaysEntries = await timeEntryQueries.GetDailyTimeEntriesForUser(user.Id, yesterday, CancellationToken.None);

            // Check conditions for notification
            bool noEntries = !yesterdaysEntries.Any();
            bool insufficientMinutes = yesterdaysEntries.Sum(e => e.Minutes) < RequiredDailyMinutes;
            int currentMinutes = yesterdaysEntries.Sum(e => e.Minutes);

            if (noEntries || insufficientMinutes)
            {
                var notificationTime = DateTime.UtcNow.Date.AddHours(HoursNumberForSendingEmail);
                if (notificationTime < DateTime.UtcNow)
                {
                    notificationTime = notificationTime.AddDays(NextDayNumber);
                }
                BackgroundJob.Schedule(
                    () => SendTimeEntryReminder(user.Email!, noEntries, insufficientMinutes, currentMinutes),
                    notificationTime - DateTime.UtcNow);
            }
        }
    }

    public void SendTimeEntryReminder(string email, bool noEntries, bool insufficientMinutes, int currentMinutes)
    {
        var model = (Email: email, NoEntries: noEntries, InsufficientMinutes: insufficientMinutes, CurrentMinutes: currentMinutes);
        var subject = "Time Entry Reminder";

        var htmlBody = emailViewRenderer.RenderView("TimeEntryReminder", model, email, subject);

        emailService.SendEmail(email, subject, htmlBody, isHtml: true);
    }
}