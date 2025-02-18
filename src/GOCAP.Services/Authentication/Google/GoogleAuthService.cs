using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GOCAP.Services;

public class GoogleAuthService(
    HttpClient _httpClient,
    IConfiguration _configuration
    ) : IGoogleAuthService
{
    public async Task<TokenResponse> ExchangeCodeForTokensAsync(string code)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _configuration["Authentication:Google:ClientId"] ?? "" },
            { "client_secret", _configuration["Authentication:Google:ClientSecret"] ?? "" },
            { "redirect_uri", $"{AppConstants.DefaultUri}{_configuration["Authentication:Google:RedirectUri"] ?? ""}" },
            { "grant_type", "authorization_code" }
        };

        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Google OAuth failed.");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        return tokenResponse ?? new();
    }

    public async Task<GoogleUser?> GetUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to retrieve user info.");
        }
        try
        {
            var userInfoContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GoogleUser>(userInfoContent);
            return result;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}");
            throw new JsonException($"JSON Error: {ex.Message}");
        }
    }
}
