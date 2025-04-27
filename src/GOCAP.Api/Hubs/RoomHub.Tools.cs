using System.Collections.Concurrent;
using static GOCAP.Api.Model.RoomPollModel;

namespace GOCAP.Api.Hubs
{
    public partial class RoomHub
    {

        private static ConcurrentDictionary<string, List<SubtitleEntry>> _subtitleCache = new();


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
            ;
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

        public async Task SummarizeSubtitles(string roomId)
        {
            if (_subtitleCache.TryGetValue(roomId, out var subtitles) && subtitles.Count > 0)
            {
                string combinedText = string.Join("\n", subtitles.Select(s => $"{s.Username}: {s.Subtitle}"));
                _logger.LogInformation("Summarizing subtitles for RoomId: {RoomId}, CombinedText: {CombinedText}", roomId, combinedText);
                string summary = await aIService.AISummaryAsync(combinedText);
                await Clients.Caller.SendAsync("ReceiveSummary", summary);
                return;
            }
            await Clients.Caller.SendAsync("ReceiveSummary", "Không có phụ đề để tóm tắt.");
        }



        public async Task CreatePoll(UserDto user, string roomId, string question, List<string> options)
        {
            TimeSpan? expireIn = null;
            if (string.IsNullOrWhiteSpace(roomId) || string.IsNullOrWhiteSpace(question) || options == null || options.Count < 2)
            {
                _logger.LogWarning("Invalid poll data: RoomId: {RoomId}, Question: {Question}, OptionsCount: {OptionsCount}", roomId, question, options?.Count);
                throw new HubException("Invalid poll data.");
            }

            _logger.LogInformation("Creating poll for RoomId: {RoomId} by User: {UserId}, Question: {Question}", roomId, user.Id, question);

            var now = DateTime.UtcNow;
            var poll = new PollModel
            {
                Question = question,
                Options = options.Select(opt => new PollOption
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = opt
                }).ToList(),
                CreatedById = user.Id,
                CreatedByName = user.Name,
                RoomId = roomId,
                CreatedAt = now,
                ExpiresAt = now.Add(expireIn ?? TimeSpan.FromHours(24))
            };

            // Add the poll to the room using the new method
            PollManager.AddPollToRoom(roomId, poll);

            _logger.LogInformation("Poll created successfully for RoomId: {RoomId}, PollId: {PollId}", roomId, poll.Id);

            // Send all polls for the room to clients
            await UpdateRoomPolls(roomId);
        }

        public async Task VoteOnPoll(UserDto user, string roomId, string pollId, string optionId)
        {
            // Get the specific poll by ID
            var poll = PollManager.GetPoll(pollId);

            if (poll == null)
            {
                throw new HubException("Poll not found.");
            }

            if (poll.RoomId != roomId)
            {
                throw new HubException("Poll does not belong to this room.");
            }

            poll.VoterNames ??= new Dictionary<string, string>();
            poll.VoterDisplayNames ??= new Dictionary<string, string>();

            var oldOptionId = poll.VoterNames.ContainsKey(user.Id) ? poll.VoterNames[user.Id] : null;

            if (oldOptionId == optionId)
                return;

            if (oldOptionId != null)
            {
                var oldOption = poll.Options.FirstOrDefault(o => o.Id == oldOptionId);
                if (oldOption != null) oldOption.Votes--;
            }
            else
            {
                poll.VoterIds.Add(user.Id);
            }

            var newOption = poll.Options.FirstOrDefault(o => o.Id == optionId);
            if (newOption == null)
                throw new HubException("Invalid option.");

            newOption.Votes++;
            poll.VoterNames[user.Id] = optionId;
            poll.VoterDisplayNames[user.Id] = user.Name;

            // Update clients with all polls for the room
            await UpdateRoomPolls(roomId);
        }

        // Helper method to send updated polls to clients
        private async Task UpdateRoomPolls(string roomId)
        {
            var polls = PollManager.GetPollsForRoom(roomId);
            await Clients.Group(roomId).SendAsync("PollUpdated", polls);
        }

        public async Task DeletePollFromRoom(string roomId, string pollId)
        {
            var success = PollManager.DeletePoll(pollId);

            if (!success)
                throw new HubException("Failed to delete poll.");

            // Gửi danh sách poll mới đến các client trong room
            await UpdateRoomPolls(roomId);
        }

        public async Task EndPollInRoom(string roomId, string pollId)
        {
            var success = PollManager.EndPoll(pollId);

            if (!success)
                throw new HubException("Failed to end poll. Maybe it's already ended?");

            await UpdateRoomPolls(roomId);
        }

        public static void RemoveExpiredPolls(DateTime currentTime)
        {
            foreach (var kvp in PollManager.AllPolls.ToList())
            {
                var poll = kvp.Value;
                if (poll.ExpiresAt.HasValue && poll.ExpiresAt.Value <= currentTime)
                {
                    PollManager.DeletePoll(kvp.Key);
                }
            }
        }
    }
}
