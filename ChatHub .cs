namespace OnlineChat
{
    using Microsoft.AspNetCore.SignalR;
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, int chatId)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message, chatId);
        }
    }
}
