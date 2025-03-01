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
    ICompositeViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider) : ITimeEntryNotificationService
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

        // Use the application’s IServiceProvider to create a scope and resolve services
        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var routeData = new Microsoft.AspNetCore.Routing.RouteData();
        var actionDescriptor = new ActionDescriptor
        {
            DisplayName = "TimeEntryReminder" // Provide a display name for debugging
        };

        // Create a minimal HttpContext with services from the application’s DI
        var httpContext = new DefaultHttpContext
        {
            RequestServices = scopedServices
        };

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        // Generate HTML content using Razor
        var viewResult = viewEngine.FindView(actionContext, "TimeEntryReminder", true); // Search shared locations (including wwwroot/Emails/)
        if (viewResult.View == null)
        {
            throw new InvalidOperationException($"Could not find the TimeEntryReminder view. Searched locations: {string.Join(", ", viewResult.SearchedLocations ?? Enumerable.Empty<string>())}");
        }

        using var writer = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider), // Use the injected ITempDataProvider
            writer,
            new HtmlHelperOptions()
        );

        // Ensure the ViewContext has access to the application’s services
        viewContext.HttpContext.RequestServices = scopedServices; // Explicitly set RequestServices to ensure all services are available

        try
        {
            viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to render the view: {ex.Message}", ex);
        }

        var htmlBody = writer.ToString();

        emailService.SendEmail(email, subject, htmlBody, isHtml: true);
    }
}