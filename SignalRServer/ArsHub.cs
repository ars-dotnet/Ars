using Microsoft.AspNetCore.SignalR;

namespace SignalRServer
{
    public class ArsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,"arsserver");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "arsserver");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
