namespace GOCAP.Domain;

public class ProviderUserBase
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
