using GOCAP.ExternalServices;

namespace GOCAP.Messaging;

public class UserLoginConsumer(
    IOptions<KafkaSettings> _kafkaSettings,
    ILogger<UserLoginConsumer> _logger, IEmailService _emailService) : KafkaConsumerBase(_kafkaSettings, _logger, KafkaConstants.Topics.UserLogin)
{
    protected override async Task ProcessMessageAsync(string message)
    {
        var mailContent = new MailContent
        {
            To = "brightsuntnc2003@gmail.com", 
            Subject = "Chào mừng bạn đến với GOCAP 🎉",
            Body = "<h1>Chào mừng bạn!</h1><p>Cảm ơn bạn đã tham gia GOCAP. Chúng tôi rất vui khi có bạn ở đây!</p>"
        };
        await _emailService.SendMailAsync(mailContent);
        _logger.LogInformation("Sent welcome email to user: {UserEmail}", mailContent.To);

    }
}

