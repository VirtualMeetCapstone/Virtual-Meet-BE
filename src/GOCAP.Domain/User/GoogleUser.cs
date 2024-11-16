namespace GOCAP.Domain;

public class GoogleUser : ProviderUserBase
{
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }
    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }
}
