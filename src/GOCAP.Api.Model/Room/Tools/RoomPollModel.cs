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
        public string RoomId { get; set; } = string.Empty; // Added RoomId to associate poll with room
        public bool IsActive { get; set; } = true;
        public DateTime? EndedAt { get; set; } = null;
        public DateTime? ExpiresAt { get; set; } = null; 

    }

    public static class PollManager
    {
        // Store polls by their ID instead of room ID
        public static ConcurrentDictionary<string, PollModel> AllPolls { get; set; } = new();

        // Map room IDs to lists of poll IDs
        public static ConcurrentDictionary<string, List<string>> RoomPolls { get; set; } = new();

        // Get all polls for a specific room
        public static List<PollModel> GetPollsForRoom(string roomId)
        {
            if (!RoomPolls.TryGetValue(roomId, out var pollIds))
            {
                return [];
            }

            return pollIds
                .Select(pollId => AllPolls.TryGetValue(pollId, out var poll) ? poll : null)
                .Where(poll => poll != null)
                .ToList();
        }

        // Add a poll to a room
        public static void AddPollToRoom(string roomId, PollModel poll)
        {
            // Add poll to AllPolls dictionary
            AllPolls[poll.Id] = poll;

            // Ensure we have a list for this room
            RoomPolls.TryGetValue(roomId, out var pollIds);
            if (pollIds == null)
            {
                pollIds = [];
                RoomPolls[roomId] = pollIds;
            }

            // Add this poll's ID to the room's poll list
            pollIds.Add(poll.Id);
        }

        // Get a specific poll by ID
        public static PollModel GetPoll(string pollId)
        {
            AllPolls.TryGetValue(pollId, out var poll);
            return poll;
        }


        public static bool DeletePoll(string pollId)
        {
            if (!AllPolls.TryGetValue(pollId, out var poll))
                return false;

            string roomId = poll.RoomId;

            // Remove from AllPolls
            AllPolls.TryRemove(pollId, out _);

            // Remove from RoomPolls
            if (RoomPolls.TryGetValue(roomId, out var pollIds))
            {
                pollIds.Remove(pollId);
            }

            return true;
        }

        // End a poll
        public static bool EndPoll(string pollId)
        {
            if (!AllPolls.TryGetValue(pollId, out var poll))
                return false;

            if (!poll.IsActive)
                return false;

            poll.IsActive = false;
            poll.EndedAt = DateTime.UtcNow;

            return true;
        }



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