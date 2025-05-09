﻿
namespace GOCAP.Repository;

[RegisterService(typeof(IRoomMemberRepository))]
internal class RoomMemberRepository(AppSqlDbContext context) : SqlRepositoryBase<RoomMemberEntity>(context), IRoomMemberRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<int> DeleteByRoomIdAsync(Guid id)
    {
        return await _context.RoomMembers
                                .Where(rm => rm.RoomId == id)
                                .ExecuteDeleteAsync();
    }

    public async Task<int> CountByRoomIdAsync(Guid roomId)
    {
        return await _context.RoomMembers
                             .CountAsync(rm => rm.RoomId == roomId);
    }


}
