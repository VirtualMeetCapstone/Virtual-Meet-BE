using System.Text.Json.Serialization;

namespace GOCAP.Api.Model
{
    public class ModerationResponse
    {

        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("badwords")]
        public List<string>? BadWords { get; set; }
    }
}
