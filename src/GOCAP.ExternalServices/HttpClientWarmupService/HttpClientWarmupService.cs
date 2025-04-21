using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GOCAP.ExternalServices
{
    public class HttpClientWarmupService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientWarmupService> _logger;
        private readonly ModerationSettings _moderationSettings;
        private readonly OpenAISettings _openAISettings;

        // Reference to the same constants used in ModerationController
        private const ModerationModel CURRENT_MODEL = ModerationModel.RapidAPI;
        private const ModerationModel CURRENT_MODEL_FOR_CHAT = ModerationModel.RapidAPI;

        private enum ModerationModel
        {
            OpenAI,
            RapidAPI,
        }

        public HttpClientWarmupService(
            IHttpClientFactory httpClientFactory,
            ILogger<HttpClientWarmupService> logger,
            ModerationSettings moderationSettings,
            OpenAISettings openAISettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _moderationSettings = moderationSettings;
            _openAISettings = openAISettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Delay slightly to ensure all services are registered
            await Task.Delay(1000, stoppingToken);

            _logger.LogInformation("Starting HTTP client connections warm-up based on configured modes...");

            // Only warm up clients that are actually used based on current mode settings

            // Check if OpenAI client is needed
            if (CURRENT_MODEL == ModerationModel.OpenAI || CURRENT_MODEL_FOR_CHAT == ModerationModel.OpenAI)
            {
                await WarmUpHttpClient("OpenAI", stoppingToken);
            }
            else
            {
                _logger.LogInformation("Skipping OpenAI client warm-up as it's not used in current configuration");
            }

            // Check if RapidAPI client is needed
            if (CURRENT_MODEL == ModerationModel.RapidAPI || CURRENT_MODEL_FOR_CHAT == ModerationModel.RapidAPI)
            {
                await WarmUpHttpClient("RapidAPI", stoppingToken);
            }
            else
            {
                _logger.LogInformation("Skipping RapidAPI client warm-up as it's not used in current configuration");
            }

            _logger.LogInformation("HTTP client connections warm-up completed");
        }

        private async Task WarmUpHttpClient(string clientName, CancellationToken cancellationToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(clientName);

                // Create a HEAD request to warm up the connection
                using var request = new HttpRequestMessage(HttpMethod.Head, "");

                _logger.LogDebug("Warming up {ClientName} connection...", clientName);

                // Set a timeout to prevent hanging if the service is down
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

                // Send the request to establish connection
                await client.SendAsync(request, linkedCts.Token);

                _logger.LogDebug("{ClientName} connection successfully warmed up", clientName);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Warm-up for {ClientName} was canceled or timed out", clientName);
            }
            catch (Exception ex)
            {
                // Log the error but don't throw, as we don't want to prevent the application from starting
                _logger.LogError(ex, "Failed to warm up {ClientName} connection: {ErrorMessage}", clientName, ex.Message);
            }
        }
    }
}