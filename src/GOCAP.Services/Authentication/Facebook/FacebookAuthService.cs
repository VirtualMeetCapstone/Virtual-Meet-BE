using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GOCAP.Services;

public class FacebookAuthService(
    HttpClient _httpClient,
    IConfiguration _configuration
    ) : IFacebookAuthService
{
    public async Task<TokenResponse> ExchangeCodeForTokensAsync(string code)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _configuration["Authentication:Facebook:AppId"] ?? "" },
            { "client_secret", _configuration["Authentication:Facebook:AppSecret"] ?? "" },
            { "redirect_uri", $"{AppConstants.DefaultUri}{_configuration["Authentication:Facebook:RedirectUri"] ?? ""}" },
            { "grant_type", "authorization_code" }
        };

        var response = await _httpClient.PostAsync("https://graph.facebook.com/v12.0/oauth/access_token", new FormUrlEncodedContent(tokenRequest));

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Facebook OAuth failed.");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        return tokenResponse ?? new();
    }

    public async Task<FacebookUser?> GetUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var fields = "id,name,first_name,last_name,email,picture,gender,locale,birthday,hometown,location,friends";
        var response = await _httpClient.GetAsync($"https://graph.facebook.com/me?fields={fields}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to retrieve user info.");
        }
        try
        {
            var userInfoContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<FacebookUser>(userInfoContent);
            return result;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}");
            throw new JsonException($"JSON Error: {ex.Message}");
        }
    }
}
