namespace GOCAP.Api.Hubs
{
    public class VideoHub : Hub
    {
        // Lưu trữ trạng thái video và popup hiện tại
        private static (string VideoId, double Timestamp, bool IsPaused) videoState = ("4Lq-I3xQxns", 0, true);
        private static Dictionary<string, bool> RoomPopupStates = new Dictionary<string, bool>();


        public async Task UpdatePlayerStatus(int status, double time)
        {
            await Clients.Others.SendAsync("receiveplayerstatus", status, time);
        }

        //public async Task ChangeVideo(string videoId, double timestamp, bool isPaused)
        //{
        //    videoState = (videoId, timestamp, isPaused);
        //    // Gửi thông báo tới tất cả client khác
        //    await Clients.Others.SendAsync("ReceiveVideo", videoId, timestamp, isPaused);
        //    Console.WriteLine($"[VideoHub] Video changed to {videoId}, Timestamp: {timestamp}, Paused: {isPaused}");
        //}

        public async Task ChangeVideo(string roomId, string videoId, double timestamp, bool isPaused)
        {
            videoState = (videoId, timestamp, isPaused);
            Console.WriteLine($"📡 Gửi video {videoId} đến Room {roomId}");

            await Clients.Group(roomId).SendAsync("ReceiveVideo", videoId, timestamp, isPaused);
        }


        public async Task SelectVideo(string videoId)
        {
            Console.WriteLine($"🎬 Video được chọn: {videoId}");
            await Clients.All.SendAsync("ReceiveSelectedVideo", videoId);
        }

        //public async Task SelectVideo(string roomId, string videoId)
        //{
        //    Console.WriteLine($"🎬 Room {roomId} - Video được chọn: {videoId}");

        //    // Debug: In danh sách ConnectionId thuộc Room này
        //    var clientsInRoom = Constant._userGroups.Where(kv => kv.Value == roomId).Select(kv => kv.Key);
        //    Console.WriteLine($"🔍 Client trong Room {roomId}: {string.Join(", ", clientsInRoom)}");
        //    await Clients.Group(roomId).SendAsync("ReceiveSelectedVideo", roomId, videoId);
        //}


    }
}
