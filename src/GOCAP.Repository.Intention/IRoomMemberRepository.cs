namespace GOCAP.Repository;

public interface IRoomMemberRepository : ISqlRepositoryBase<RoomMember>
{
    Task<int> DeleteByRoomIdAsync(Guid id);
}

