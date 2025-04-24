
using DocumentFormat.OpenXml.Drawing;
using FluentAssertions.Extensions;
using GOCAP.Database;
using MongoDB.Driver;

namespace GOCAP.Api.Hubs
{
    public partial class RoomHub
    {

        private async Task UpdateRoomStatisticsOnJoinAsync(string roomId, UserInfo user,
      IRoomService roomService, AppMongoDbContext dbContext, IMapper mapper)
        {
            Guid idGuild = Guid.Parse(roomId);
            // Use the scoped services passed as parameters instead of injected ones
            RoomStatistics stat;
            if (!RoomStateManager.RoomStats.ContainsKey(roomId))
            {
                var roomInfo = await roomService.GetDetailByIdAsync(idGuild);
                var startTime = new DateTime(roomInfo.CreateTime, DateTimeKind.Utc);
                _logger.LogInformation("🕒 Room '{RoomId}' started at {Time}", roomId, startTime, "roomInfo.Owner.CreateTime -", roomInfo.CreateTime);
                var newStat = new RoomStatistics
                {
                    RoomId = roomInfo.Id.ToString(),
                    RoomTopic = roomInfo.Topic,
                    OwnerId = roomInfo.OwnerId.ToString(),
                    OwnerName = roomInfo.Owner?.Name ?? "Unknown",
                    StartTime = startTime,
                    PeakUsers = 0,
                    PeakTime = null,
                };
                RoomStateManager.RoomStats[roomId] = newStat;
                var entity = mapper.Map<RoomStatisticsEntity>(newStat);
                entity.Id = Guid.NewGuid(); // Đảm bảo có ID mới
                await dbContext.RoomStatistic.InsertOneAsync(entity);
            }
            stat = RoomStateManager.RoomStats[roomId];
            stat.UserJoined(user);
            // Tính số người hiện tại
            var currentUserCount = stat.JoinLogs.Count - stat.LeaveLogs.Count;
            // Cập nhật peak nếu cần
            if (currentUserCount > stat.PeakUsers)
            {
                stat.PeakUsers = currentUserCount;
                stat.PeakTime = DateTime.UtcNow;
            }
            _logger.LogInformation("👤 User '{UserName}' (ID: {UserId}) joined room '{RoomId}' at {Time}",
                user.Name, user.Id, roomId, DateTime.UtcNow);
            if (currentUserCount == stat.PeakUsers && stat.PeakTime != null)
            {
                _logger.LogInformation("📈 Room '{RoomId}' at PEAK ({Count} users) since {Time}",
                    roomId, stat.PeakUsers, stat.PeakTime);
            }
            // Cập nhật lại Mongo DB document
            var filter = Builders<RoomStatisticsEntity>.Filter.Eq(x => x.RoomId, roomId);
            var update = Builders<RoomStatisticsEntity>.Update
                .Set(x => x.JoinLogs, stat.JoinLogs)
                .Set(x => x.LeaveLogs, stat.LeaveLogs)
                .Set(x => x.PeakUsers, stat.PeakUsers)
                .Set(x => x.PeakTime, stat.PeakTime);
            await dbContext.RoomStatistic.UpdateOneAsync(filter, update);
        }
        private async Task UpdateRoomStatisticsOnLeaveAsync(string roomId, UserInfo user)
        {
            if (!RoomStateManager.RoomStats.ContainsKey(roomId))
            {
                _logger.LogWarning("⚠️ User '{UserName}' (ID: {UserId}) attempted to leave room '{RoomId}' which doesn't have statistics",
                    user.Name, user.Id, roomId);
                return;
            }

            var stat = RoomStateManager.RoomStats[roomId];

            // Sử dụng phương thức UserLeft đã được cải tiến
            stat.UserLeft(user);

            _logger.LogInformation("👤 User '{UserName}' (ID: {UserId}) left room '{RoomId}' at {Time}",
                user.Name, user.Id, roomId, DateTime.UtcNow);

            var currentUserCount = stat.GetCurrentUserCount();
            if (currentUserCount == 0 && stat.EndTime != null)
            {
                _logger.LogInformation("🏁 Room '{RoomId}' is now empty, session ended at {Time}",
                    roomId, stat.EndTime);
            }

            // Cập nhật lại Mongo DB document
            var filter = Builders<RoomStatisticsEntity>.Filter.Eq(x => x.RoomId, roomId);
            var update = Builders<RoomStatisticsEntity>.Update
                .Set(x => x.StartTime, stat.StartTime)
                .Set(x => x.LeaveLogs, stat.LeaveLogs)
                .Set(x => x.EndTime, stat.EndTime);
            await _dbContext.RoomStatistic.UpdateOneAsync(filter, update);
        }
    }

}
