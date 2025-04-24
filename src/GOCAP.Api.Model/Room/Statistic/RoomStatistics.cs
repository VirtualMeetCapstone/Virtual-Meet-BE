using GOCAP.Database;

namespace GOCAP.Api.Model;

public class RoomStatistics
{
    public string RoomId { get; set; }
    public string RoomTopic { get; set; }
    public string OwnerId { get; set; }
    public string OwnerName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public List<UserLogEntry> JoinLogs { get; set; } = new();
    public List<UserLogEntry> LeaveLogs { get; set; } = new();


    public int PeakUsers { get; set; } = 0;
    public DateTime? PeakTime { get; set; }

    public void UserJoined(UserInfo user)
    {
        var now = DateTime.UtcNow;

        // Kiểm tra xem người dùng đã từng vào phòng trước đó hay chưa
        var lastJoinLog = JoinLogs.LastOrDefault(j => j.User.Id == user.Id);
        var lastLeaveLog = LeaveLogs.LastOrDefault(l => l.User.Id == user.Id);

        // Chỉ thêm log mới khi người dùng chưa ở trong phòng
        bool userAlreadyInRoom = false;
        if (lastJoinLog != null && lastLeaveLog != null)
        {
            // Nếu thời gian join cuối > thời gian leave cuối, nghĩa là user đã ở trong phòng
            userAlreadyInRoom = lastJoinLog.Time > lastLeaveLog.Time;
        }
        else if (lastJoinLog != null && lastLeaveLog == null)
        {
            // Có log join nhưng không có log leave, nghĩa là user đã ở trong phòng
            userAlreadyInRoom = true;
        }

        if (!userAlreadyInRoom)
        {
            // Chỉ thêm log và cập nhật thống kê khi user thực sự mới vào
            JoinLogs.Add(new UserLogEntry { User = user, Time = now });

            if (StartTime == null)
                StartTime = now;

            var currentUserCount = GetCurrentUserCount();
            if (currentUserCount > PeakUsers)
            {
                PeakUsers = currentUserCount;
                PeakTime = now;
            }
        }
    }

    public void UserLeft(UserInfo user)
    {
        var now = DateTime.UtcNow;

        // Kiểm tra xem người dùng có thực sự đang ở trong phòng hay không
        var lastJoinLog = JoinLogs.LastOrDefault(j => j.User.Id == user.Id);
        var lastLeaveLog = LeaveLogs.LastOrDefault(l => l.User.Id == user.Id);

        bool userInRoom = false;
        if (lastJoinLog != null)
        {
            if (lastLeaveLog == null)
            {
                // Có join log nhưng không có leave log
                userInRoom = true;
            }
            else
            {
                // Nếu thời gian join cuối > thời gian leave cuối, nghĩa là user đang ở trong phòng
                userInRoom = lastJoinLog.Time > lastLeaveLog.Time;
            }
        }

        if (userInRoom)
        {
            LeaveLogs.Add(new UserLogEntry { User = user, Time = now });

            var currentUserCount = GetCurrentUserCount();
            if (currentUserCount == 0)
                EndTime = now;
        }
    }

    // Helper method để tính số người dùng hiện tại trong phòng
    public int GetCurrentUserCount()
    {
        int count = 0;
        var userIds = new HashSet<string>();

        // Đếm số người độc nhất đang trong phòng
        foreach (var joinLog in JoinLogs)
        {
            var userId = joinLog.User.Id;
            var joinTime = joinLog.Time;

            // Tìm thời gian leave gần nhất của user này
            var lastLeaveTime = LeaveLogs
                .Where(l => l.User.Id == userId)
                .OrderByDescending(l => l.Time)
                .Select(l => l.Time)
                .FirstOrDefault();

            // Nếu thời gian join > thời gian leave gần nhất (hoặc chưa có leave log), user đang trong phòng
            if (lastLeaveTime == default || joinTime > lastLeaveTime)
            {
                if (!userIds.Contains(userId))
                {
                    userIds.Add(userId);
                    count++;
                }
            }
        }

        return count;
    }
}

