using Newtonsoft.Json;

namespace GOCAP.Api.Model;

public class RoomPlaybackStateModel
{
    [JsonProperty("videoId")]
    public string VideoId { get; set; } = "";
    [JsonProperty("time")]
    public double Time { get; set; }
    [JsonProperty("isPaused")]
    public bool IsPaused { get; set; } = true;
    [JsonProperty("sharing")]
    public bool Sharing { get; set; } = false;
}
