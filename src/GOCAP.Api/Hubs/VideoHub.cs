namespace GOCAP.Api.Hubs
{
    public class VideoHub : Hub
    {
        // Lưu trữ trạng thái video và popup hiện tại
        private static (string VideoId, double Timestamp, bool IsPaused) videoState = ("4Lq-I3xQxns", 0, true);
        private static Dictionary<string, bool> RoomPopupStates = new Dictionary<string, bool>();

        //b1: gui trang thai popUP
        public async Task TogglePopup(bool isOpen, string roomId)
        {
            if (!string.IsNullOrEmpty(roomId))
            {
                RoomPopupStates[roomId] = isOpen; // ✅ Cập nhật trạng thái đúng cách

                Console.WriteLine($"[VideoHub] 🔄 TogglePopup: Room {roomId} - isOpen={isOpen}");

                await Clients.Group(roomId).SendAsync("ReceivePopupState", isOpen);
                Console.WriteLine($"[VideoHub] ✅ Gửi event popup đến Group {roomId}");
            }
            else
            {
                Console.WriteLine("[VideoHub] ❌ Lỗi: roomId rỗng");
            }
        }



        public Task<bool> GetPopupState(string roomId)
        {
            if (RoomPopupStates.TryGetValue(roomId, out bool isOpen))
            {
                Console.WriteLine($"[VideoHub] 🟢 GetPopupState({roomId}) = {isOpen}");
                return Task.FromResult(isOpen);
            }

            Console.WriteLine($"[VideoHub] 🔴 GetPopupState({roomId}): Default to false");
            return Task.FromResult(false); // Mặc định là false nếu chưa có dữ liệu
        }


        public async Task UpdatePlayerStatus(int status, double time)
        {
            await Clients.Others.SendAsync("receiveplayerstatus", status, time);
        }

        public async Task ChangeVideo(string videoId, double timestamp, bool isPaused)
        {
            videoState = (videoId, timestamp, isPaused);
            // Gửi thông báo tới tất cả client khác
            await Clients.Others.SendAsync("ReceiveVideo", videoId, timestamp, isPaused);
            Console.WriteLine($"[VideoHub] Video changed to {videoId}, Timestamp: {timestamp}, Paused: {isPaused}");
        }

        // Lấy trạng thái video hiện tại và gửi lại cho client yêu cầu
        public async Task SelectVideo(string videoId)
        {
            Console.WriteLine($"🎬 Video được chọn: {videoId}");
            await Clients.All.SendAsync("ReceiveSelectedVideo", videoId); // Gửi video đến tất cả client
        }
    }
}