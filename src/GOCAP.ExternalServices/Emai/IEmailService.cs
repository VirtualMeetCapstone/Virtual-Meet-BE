using GOCAP.ExternalServices.Models;

namespace GOCAP.ExternalServices;

public interface IEmailService
{
	Task<bool> SendMailAsync(MailContent mailContent);
}
