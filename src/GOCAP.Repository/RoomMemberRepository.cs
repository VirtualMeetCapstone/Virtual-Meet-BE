
namespace GOCAP.Repository;

[RegisterService(typeof(IRoomMemberRepository))]
internal class RoomMemberRepository(
    AppSqlDbContext context,
     IMapper mapper) : SqlRepositoryBase<RoomMember, RoomMemberEntity>(context, mapper), IRoomMemberRepository
{
    private readonly AppSqlDbContext _context = context;
    public async Task<int> DeleteByRoomIdAsync(Guid id)
    {
        return await _context.RoomMembers
                                .Where(rm => rm.RoomId == id)
                                .ExecuteDeleteAsync();
    }
}
