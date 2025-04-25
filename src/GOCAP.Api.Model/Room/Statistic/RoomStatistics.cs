using GOCAP.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GOCAP.Api.Model;

public class RoomStatistics
{
    public string RoomId { get; set; }
    public string RoomTopic { get; set; }
    public string OwnerId { get; set; }
    public string OwnerName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int TotalJoins => JoinLogs.Count;
    public int TotalLeaves => LeaveLogs.Count;



    public List<UserLogEntry> JoinLogs { get; set; } = new();
    public List<UserLogEntry> LeaveLogs { get; set; } = new();

    public int PeakUsers { get; set; } = 0;
    public DateTime? PeakTime { get; set; }

    // Tổng số người dùng khác nhau đã tham gia phòng
    public int TotalUniqueUsers => JoinLogs.Select(log => log.User.Id).Distinct().Count();
    public int CurrentUsersCount => GetCurrentUserCount();
   
    // Thời lượng tồn tại của phòng
    public TimeSpan? Duration => (EndTime ?? DateTime.UtcNow) - StartTime;

    // Thời gian trung bình một người ở lại phòng
    public TimeSpan? AverageUserSessionTime
    {
        get
        {
            var totalDuration = TimeSpan.Zero;
            var userIds = JoinLogs.Select(j => j.User.Id).Distinct();

            foreach (var userId in userIds)
            {
                var joins = JoinLogs.Where(j => j.User.Id == userId).OrderBy(j => j.Time).ToList();
                var leaves = LeaveLogs.Where(l => l.User.Id == userId).OrderBy(l => l.Time).ToList();

                for (int i = 0; i < Math.Min(joins.Count, leaves.Count); i++)
                {
                    totalDuration += leaves[i].Time - joins[i].Time;
                }
            }

            return userIds.Any() ? (TimeSpan?)TimeSpan.FromTicks(totalDuration.Ticks / userIds.Count()) : null;
        }
    }

    // Thời gian user đầu tiên join phòng
    public DateTime? FirstJoinTime => JoinLogs.Any() ? JoinLogs.Min(j => j.Time) : (DateTime?)null;

    // Thời lượng phòng có người tham gia (từ user đầu tiên join)
    public TimeSpan? DurationFromFirstJoin => FirstJoinTime.HasValue
        ? (EndTime ?? DateTime.UtcNow) - FirstJoinTime
        : null;

    // Số lần mỗi người vào - ra phòng
    public Dictionary<string, int> UserSessionCounts
    {
        get
        {
            var result = new Dictionary<string, int>();
            var userIds = JoinLogs.Select(j => j.User.Id).Union(LeaveLogs.Select(l => l.User.Id)).Distinct();

            foreach (var userId in userIds)
            {
                var joins = JoinLogs.Where(j => j.User.Id == userId).OrderBy(j => j.Time).ToList();
                var leaves = LeaveLogs.Where(l => l.User.Id == userId).OrderBy(l => l.Time).ToList();

                // Số session là số cặp Join–Leave hợp lệ
                int sessionCount = Math.Min(joins.Count, leaves.Count);
                result[userId] = sessionCount;
            }

            return result;
        }
    }


    public double UserParticipationRate =>
    TotalUniqueUsers == 0 ? 0 : (double)CurrentUsersCount / TotalUniqueUsers;

    public string FirstJoinUserId => JoinLogs.OrderBy(j => j.Time).FirstOrDefault()?.User.Id;

    public string LastJoinUserId => JoinLogs.OrderByDescending(j => j.Time).FirstOrDefault()?.User.Id;

    public TimeSpan? LongestSessionTime
    {
        get
        {
            TimeSpan maxDuration = TimeSpan.Zero;
            var userIds = JoinLogs.Select(j => j.User.Id).Distinct();

            foreach (var userId in userIds)
            {
                var joins = JoinLogs.Where(j => j.User.Id == userId).OrderBy(j => j.Time).ToList();
                var leaves = LeaveLogs.Where(l => l.User.Id == userId).OrderBy(l => l.Time).ToList();

                for (int i = 0; i < Math.Min(joins.Count, leaves.Count); i++)
                {
                    var sessionDuration = leaves[i].Time - joins[i].Time;
                    if (sessionDuration > maxDuration)
                        maxDuration = sessionDuration;
                }
            }

            return maxDuration == TimeSpan.Zero ? (TimeSpan?)null : maxDuration;
        }
    }

    public void UserJoined(UserInfo user)
    {
        var now = DateTime.UtcNow;

        // Kiểm tra xem user đã vào nhưng chưa rời chưa
        var lastJoin = JoinLogs.LastOrDefault(j => j.User.Id == user.Id);
        var lastLeave = LeaveLogs.LastOrDefault(l => l.User.Id == user.Id);

        bool isCurrentlyInRoom = lastJoin != null && (lastLeave == null || lastJoin.Time > lastLeave.Time);
        if (isCurrentlyInRoom)
            return; // Tránh thêm log thừa nếu user đã trong phòng

        // Thêm log vào phòng
        JoinLogs.Add(new UserLogEntry { User = user, Time = now });

        // Nếu là người đầu tiên, cập nhật thời gian bắt đầu
        if (StartTime == null)
            StartTime = now;

        // Cập nhật số người hiện tại và kiểm tra peak
        int currentUsers = GetCurrentUserCount();
        if (currentUsers > PeakUsers)
        {
            PeakUsers = currentUsers;
            PeakTime = now;
        }

        // Nếu có người vào lại sau khi phòng đã kết thúc trước đó, reset EndTime
        if (EndTime != null)
            EndTime = null;
    }


    public void UserLeft(UserInfo user)
    {
        var now = DateTime.UtcNow;

        var lastJoin = JoinLogs.LastOrDefault(j => j.User.Id == user.Id);
        var lastLeave = LeaveLogs.LastOrDefault(l => l.User.Id == user.Id);

        bool isInRoom = lastJoin != null && (lastLeave == null || lastJoin.Time > lastLeave.Time);
        if (!isInRoom)
            return; // Nếu không có trong phòng thì bỏ qua

        // Thêm log rời phòng
        LeaveLogs.Add(new UserLogEntry { User = user, Time = now });

        // Nếu không còn người nào trong phòng -> cập nhật thời gian kết thúc
        if (GetCurrentUserCount() == 0)
            EndTime = now;
    }


    // Tính số người đang trong phòng
    public int GetCurrentUserCount()
    {
        int count = 0;
        var userIds = new HashSet<string>();

        foreach (var joinLog in JoinLogs)
        {
            var userId = joinLog.User.Id;
            var joinTime = joinLog.Time;

            var lastLeaveTime = LeaveLogs
                .Where(l => l.User.Id == userId)
                .OrderByDescending(l => l.Time)
                .Select(l => l.Time)
                .FirstOrDefault();

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
