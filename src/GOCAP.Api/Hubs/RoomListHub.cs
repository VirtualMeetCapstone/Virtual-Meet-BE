namespace GOCAP.Api.Hubs;

public class RoomListHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "RoomsPage");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "RoomsPage");
        await base.OnDisconnectedAsync(exception);
    }
}
