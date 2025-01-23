namespace GOCAP.Services;

[RegisterService(typeof(IRoomService))]
internal class RoomService(
    IRoomRepository _repository,
    IRoomMemberRepository _roomMemberRepository,
    IUnitOfWork _unitOfWork,
    ILogger<RoomService> _logger
    ) : ServiceBase<Room>(_repository, _logger), IRoomService
{
    public override async Task<Room> AddAsync(Room room)
    {
        _logger.LogInformation("Start adding a new entity of type {EntityType}.", typeof(Room).Name);
        room.CreateTime = DateTime.Now.Ticks;
        return await _repository.AddAsync(room);
    }
    public override async Task<OperationResult> DeleteByIdAsync(Guid id)
    {
        _logger.LogInformation("Start deleting entity of type {EntityType}.", typeof(Room).Name);
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _roomMemberRepository.DeleteByRoomIdAsync(id);
            var result = await _repository.DeleteByIdAsync(id);

            await _unitOfWork.CommitTransactionAsync();
            return new OperationResult(result > 0);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Error occurred while deleting entity of type {EntityType}.", typeof(Room).Name);
            await _unitOfWork.RollbackTransactionAsync();
            return new OperationResult(false);
        }
    }

}