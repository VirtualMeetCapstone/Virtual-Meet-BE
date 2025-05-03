namespace GOCAP.Api.Hubs
{
    public class StatusHub : Hub
    {
        public async Task UpdateStatus()
        {
            await Clients.All.SendAsync("Just updated status");
        }
   
        
    }
}
