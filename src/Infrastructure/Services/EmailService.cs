using Application.Common.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    private readonly string _smtpHost = configuration["EmailSettings:SmtpHost"];
    private readonly int _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
    private readonly string _smtpUser = configuration["EmailSettings:SenderEmail"];
    private readonly string _smtpPassword = configuration["EmailSettings:SenderPassword"];
    private readonly string _fromEmail = configuration["EmailSettings:SenderEmail"];
    private readonly string _senderName = configuration["EmailSettings:SenderName"];

    public async Task SendEmail(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            // Create the MimeMessage
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _fromEmail));
            message.To.Add(new MailboxAddress("Recipient", to));
            message.Subject = subject;

            // Create the body of the email
            message.Body = new TextPart(isHtml ? "html" : "plain")
            {
                Text = body
            };

            // Connect to the SMTP server and send the email
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_smtpUser, _smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            logger.LogInformation($"Email sent to {to}: {subject}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to send email to {to}: {ex.Message}");
        }
    }

    public Task SendEmail(string to, string subject, string body)
    {
        throw new NotImplementedException();
    }
}