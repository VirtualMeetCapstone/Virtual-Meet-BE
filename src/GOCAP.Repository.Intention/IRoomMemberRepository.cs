namespace GOCAP.Repository;

public interface IRoomMemberRepository : ISqlRepositoryBase<RoomMemberEntity>
{
    Task<int> DeleteByRoomIdAsync(Guid id);
    Task<int> CountByRoomIdAsync(Guid roomId);

}

