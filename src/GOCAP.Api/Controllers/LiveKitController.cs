using Livekit.Server.Sdk.Dotnet;

namespace GOCAP.Api.Controllers;

[Route("livekit")]
public class LiveKitController(LiveKitSettings settings) : ApiControllerBase
{
    private readonly LiveKitSettings _settings = settings;

    [HttpPost("token")]
    public ActionResult<string> GenerateToken([FromBody] RoomLiveKitModel request)
    {
        var token = new AccessToken(_settings.ApiKey, _settings.ApiSecret)
            .WithIdentity(request.ParticipantName)
            .WithGrants(new VideoGrants { RoomJoin = true, Room = request.RoomName });

        return token.ToJwt();
    }
}
