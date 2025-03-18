using GOCAP.Common;
using GOCAP.Domain;
using GOCAP.ExternalServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GOCAP.Messaging.Consumer;

public class UserLoginConsumer(
    IOptions<KafkaSettings> _kafkaSettings,
    IAppConfiguration _appConfiguration,
    ILogger<UserLoginConsumer> _logger, IEmailService _emailService) : KafkaConsumerBase(_kafkaSettings, _logger, KafkaConstants.Topics.UserLogin)
{

    protected override async Task ProcessMessageAsync(string message)
    {
        var userLoginEvent = JsonHelper.Deserialize<UserLoginEvent>(message);
        if (userLoginEvent != null)
        {
            var mailContent = new EmailContent
            {
                To = userLoginEvent.Email,
                Subject = "🌟 Welcome to GOCAP - Connect and Explore! 🎉",
                Body = WelcomeEmailBody(userLoginEvent.Username)
            };
            await _emailService.SendMailAsync(mailContent);
            _logger.LogInformation("Sent welcome email to user: {UserEmail}", mailContent.To);
        }
        await Task.CompletedTask;
    }

    private string WelcomeEmailBody(string username)
    => $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }}
                .container {{
                    max-width: 600px;
                    margin: 20px auto;
                    background: #ffffff;
                    padding: 30px;
                    border-radius: 10px;
                    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    text-align: center;
                    padding: 20px 0;
                    background: linear-gradient(135deg, #6a11cb, #2575fc);
                    color: white;
                    border-radius: 10px 10px 0 0;
                }}
                .header h1 {{
                    margin: 0;
                    font-size: 26px;
                }}
                .content {{
                    padding: 20px;
                    text-align: center;
                    color: #333;
                }}
                .content h2 {{
                    color: #6a11cb;
                    font-size: 22px;
                }}
                .button {{
                    display: inline-block;
                    margin-top: 20px;
                    padding: 12px 24px;
                    font-size: 16px;
                    font-weight: bold;
                    color: white;
                    background-color: #2575fc;
                    border-radius: 5px;
                    text-decoration: none;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                    text-align: center;
                }}
                .footer a {{
                    color: #2575fc;
                    text-decoration: none;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>🎉 Welcome to GOCAP! 🚀</h1>
                </div>
                <div class='content'>
                    <h2>Hello, {username}! 👋</h2>
                    <p>
                        We’re absolutely thrilled to welcome you to <strong>GOCAP</strong> – 
                        the next-generation social networking platform where you can 
                        connect, explore, and engage with like-minded individuals from around the world.
                    </p>
                    <p>
                        At GOCAP, we believe in creating meaningful experiences and fostering 
                        a dynamic community. Whether you're here to meet new friends, 
                        share your passions, or build something amazing, we’re excited to 
                        have you on board.
                    </p>
                    <p>
                        Here’s what you can do next:
                    </p>
                    <ul style='text-align: left; margin: 0 auto; max-width: 500px;'>
                        <li>🌍 Explore trending topics and discussions.</li>
                        <li>💬 Join rooms and engage in live conversations.</li>
                        <li>📸 Share your moments with a vibrant community.</li>
                        <li>🚀 Customize your profile and build your network.</li>
                    </ul>
                    <p>
                        Ready to get started? Click the button below to begin your journey!
                    </p>
                    <a href='{_appConfiguration.GetDefaultDomain()}' class='button'>Start Exploring 🚀</a>
                </div>
                <div class='footer'>
                    <p>
                        If you have any questions or need assistance, feel free to reach out to us at 
                        <a href='mailto:support@gocap.com'>support@gocap.com</a>. We're always here to help!
                    </p>
                    <p>&copy; 2025 GOCAP. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

}

