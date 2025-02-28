namespace GOCAP.Services.Intention;

public interface IMessageService : IServiceBase<Message>
{
    Task<RoomMessage> AddRoomMessageAsync(RoomMessage domain);
}
