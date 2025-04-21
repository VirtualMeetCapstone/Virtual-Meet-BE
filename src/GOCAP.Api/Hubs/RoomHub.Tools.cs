using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace GOCAP.Api.Hubs
{
    public partial class RoomHub
    {

        private static ConcurrentDictionary<string, List<SubtitleEntry>> _subtitleCache = new();
        private Timer _flushTimer;

        // Your model
        public class SubtitleEntry
        {
            public string Username { get; set; }
            public string Subtitle { get; set; }
        }
        public async Task SendShare()
        {
            if (RoomStateManager.Users.TryGetValue(Context.ConnectionId, out RoomConnectedUserModel? user))
            {
                _logger.LogInformation("🔁 [SHARE] {User} shared in Room {RoomId}", user.Name, user.RoomId);
                RoomStateManager.SharingUsers.TryAdd(user.RoomId, true);
                await Clients.Group(user.RoomId).SendAsync("ReceiveShare", user.Name);
            }
            else
            {
                _logger.LogError("❌ [ERROR] SendShare failed - User not found for ConnectionId {ConnectionId}", Context.ConnectionId);
            }
        }

        public async Task SendRaiseHand(string username, string roomId)
        {
            Console.WriteLine($"📢 Emotion Sent - User: {username}, Room: {roomId}");

            await Clients.OthersInGroup(roomId).SendAsync("ReceiveRaiseHand", username);
        }

        public async Task SendLowerHand(string username, string roomId)
        {
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveLowerHand", username);
        }

        public async Task SendEmotion(string username, string roomId, string type, double x, double y)
        {
            _logger.LogInformation("Sent - User: {Username}, Room: {RoomId}, Type: {Type}, Position: ({X}, {Y})",
                username, roomId, type, x, y);
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveEmotion", username, type, x, y);
        }

        public async Task SendSubtitle(string roomId, string username, string subtitle, string sourceLang)
        {
            await Clients.Group(roomId).SendAsync("ReceiveSubtitle", username, subtitle, sourceLang);
            var subtitleData = new SubtitleEntry
            {
                Username = username,
                Subtitle = subtitle,
            };

            // Ensure thread-safe update and check for null
            _ = _subtitleCache.AddOrUpdate(roomId,
                new List<SubtitleEntry> { subtitleData },
                (key, existingList) =>
                {
                    if (existingList == null)
                    {
                        existingList = [];
                    }

                    existingList.Add(subtitleData);
                    return existingList;
                });

        }

        public async Task<string> GetRoomSubtitles(string roomId)
        {
            _logger.LogInformation("Fetching subtitles for room {RoomId}", roomId);

            if (_subtitleCache.TryGetValue(roomId, out var subtitles) && subtitles != null)
            {
                string serializedSubtitles = JsonConvert.SerializeObject(subtitles);
                return serializedSubtitles;
            }
            return JsonConvert.SerializeObject(new List<SubtitleEntry>());
        }





    }
}
