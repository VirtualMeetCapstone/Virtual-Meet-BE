using GOCAP.Database;
using GOCAP.Database.MongoDb_Entities;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace GOCAP.Api.Hubs
{
    public class WhiteBoardHub : Hub
    {
        private readonly AppMongoDbContext _dbContext;

        public WhiteBoardHub(AppMongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendDrawingAction(string roomId, DrawingAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // Get or create whiteboard for the room
            var whiteboard = await _dbContext.WhiteBoards
                .Find(w => w.RoomId == roomId)
                .FirstOrDefaultAsync();

            if (whiteboard == null)
            {
                whiteboard = new WhiteBoardEntity
                {
                    RoomId = roomId,
                    DrawActions = new List<DrawingAction>()
                };
                await _dbContext.WhiteBoards.InsertOneAsync(whiteboard);
            }

            // Add action to history
            whiteboard.DrawActions.Add(action);
            await _dbContext.WhiteBoards.ReplaceOneAsync(
                w => w.Id == whiteboard.Id,
                whiteboard);

            // Send to other clients in the same room
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveDrawingAction", action);
        }

        public async Task ClearWhiteboard(string roomId)
        {
            var whiteboard = await _dbContext.WhiteBoards
                .Find(w => w.RoomId == roomId)
                .FirstOrDefaultAsync();

            if (whiteboard != null)
            {
                whiteboard.DrawActions.Clear();
                await _dbContext.WhiteBoards.ReplaceOneAsync(
                    w => w.Id == whiteboard.Id,
                    whiteboard);
                await Clients.Group(roomId).SendAsync("WhiteboardCleared");
            }
        }

        public async Task RequestDrawingHistory(string roomId)
        {
            var whiteboard = await _dbContext.WhiteBoards
                .Find(w => w.RoomId == roomId)
                .FirstOrDefaultAsync();

            if (whiteboard != null)
            {
                await Clients.Caller.SendAsync("LoadDrawingActions", whiteboard.DrawActions.ToList());
            }
        }

        public async Task JoinRoom(string roomId)
        {
            // Add client to the room group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // Send drawing history when joining
            await RequestDrawingHistory(roomId);
        }

        public async Task LeaveRoom(string roomId)
        {
            // Remove client from the room group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}