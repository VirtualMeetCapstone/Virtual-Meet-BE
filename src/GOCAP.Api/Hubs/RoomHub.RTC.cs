namespace GOCAP.Api.Hubs
{
    public partial class RoomHub
    {
        public async Task SendOffer(string targetPeerId, string offer)
        {
            // Find the sender's room and username
            string senderName = "Anonymous";
            foreach (var room in RoomStateManager.RoomPeers)
            {
                var peer = room.Value.FirstOrDefault(p => p.PeerId == Context.ConnectionId);
                if (peer != null)
                {
                    senderName = peer.UserName ?? "";
                    break;
                }
            }

            await Clients.Client(targetPeerId).SendAsync(
                "ReceiveOffer",
                Context.ConnectionId,
                senderName,
                offer
            );
        }
        public async Task RequestStream(string targetPeerId)
        {
            await Clients.Client(targetPeerId).SendAsync("ReceiveStreamRequest", Context.ConnectionId);
        }


        public async Task SendAnswer(string targetPeerId, string answer)
        {
            await Clients.Client(targetPeerId).SendAsync(
                "ReceiveAnswer",
                Context.ConnectionId,
                answer
            );
        }

        public async Task SendCandidate(string targetPeerId, string candidate)
        {
            await Clients.Client(targetPeerId).SendAsync(
                "ReceiveCandidate",
                Context.ConnectionId,
                candidate
            );
        }

    }
}
