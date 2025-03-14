namespace GOCAP.ExternalServices;

public interface IEmailService
{
    Task<bool> SendMailAsync(MailContent mailContent);
}
