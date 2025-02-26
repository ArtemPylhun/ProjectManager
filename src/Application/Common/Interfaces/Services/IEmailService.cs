namespace Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task SendEmail(string to, string subject, string body);
}