namespace GOCAP.Api.Hubs
{
    public class QuizHub : Hub
    {
        public async Task AddToGroup(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        public async Task JoinQuiz(string roomId)
        {
            await Clients.Group(roomId).SendAsync("Join successful");
        }

        public async Task StartQuiz(string roomId)
        {
            await Clients.Group(roomId).SendAsync("Start quiz");
        }
        public async Task ChooseQuiz(string roomId,string topic,string sessionQuiz)
        {
            await Clients.Group(roomId).SendAsync("Quiz Update",topic,sessionQuiz);
        }
        public async Task SelectAsnwer(string roomId)
        {
            await Clients.Group(roomId).SendAsync("Someone just answered the question");
        }
        public async Task closeQuiz(string roomId)
        {
            await Clients.Group(roomId).SendAsync("Close Quiz");
        }

    }
}
