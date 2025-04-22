using System.Collections.Concurrent;

namespace GOCAP.Api.Model;

public class RoomPollModel
{
    public class PollModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Question { get; set; } = string.Empty;
        public List<PollOption> Options { get; set; } = [];
        public List<string> VoterIds { get; set; } = [];
        public Dictionary<string, string> VoterNames { get; set; } = new();
        public Dictionary<string, string> VoterDisplayNames { get; set; } = new();
        public string CreatedById { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public static class PollManager
    {
        public static ConcurrentDictionary<string, PollModel> ActivePolls { get; set; } = new();
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class PollOption
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; } = string.Empty;
        public int Votes { get; set; } = 0;
    }


}
