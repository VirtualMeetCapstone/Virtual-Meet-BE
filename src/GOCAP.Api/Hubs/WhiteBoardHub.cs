using GOCAP.Database.MongoDb_Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GOCAP.Api.Hubs
{
    public class WhiteBoardHub : Hub
    {
        private static readonly ConcurrentDictionary<string, WhiteBoardEntity> _whiteboards = new();

        public async Task SendDrawingAction(string roomId, DrawingAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            WhiteBoardEntity whiteboard;
            if (_whiteboards.ContainsKey(roomId))
            {
                whiteboard = _whiteboards[roomId];
            }
            else
            {
                whiteboard = new WhiteBoardEntity
                {
                    RoomId = roomId,
                    DrawActions = new List<DrawingAction>()
                };

                _whiteboards[roomId] = whiteboard;
            }

            lock (whiteboard.DrawActions)
            {
                whiteboard.DrawActions.Add(action);
            }

            await Clients.OthersInGroup(roomId).SendAsync("ReceiveDrawingAction", action);
        }


        public async Task ClearWhiteboard(string roomId)
        {
            if (_whiteboards.TryGetValue(roomId, out var whiteboard))
            {
                lock (whiteboard.DrawActions)
                {
                    whiteboard.DrawActions.Clear();
                }
                await Clients.Group(roomId).SendAsync("WhiteboardCleared");
            }
        }

        public async Task RequestDrawingHistory(string roomId)
        {
            if (_whiteboards.TryGetValue(roomId, out var whiteboard))
            {
                List<DrawingAction> actionsCopy;
                lock (whiteboard.DrawActions)
                {
                    actionsCopy = whiteboard.DrawActions.ToList();
                }
                await Clients.Caller.SendAsync("LoadDrawingActions", actionsCopy);
            }
        }

        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task LeaveRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
    }
}