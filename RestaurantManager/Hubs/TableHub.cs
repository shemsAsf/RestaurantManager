using Microsoft.AspNetCore.SignalR;

namespace RestaurantManager.Hubs
{
    public class TableHub : Hub
    {
        public async Task JoinTable(string tableNumber) =>
            await Groups.AddToGroupAsync(Context.ConnectionId, $"table-{tableNumber}");

        public async Task LeaveTable(string tableNumber) =>
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"table-{tableNumber}");
    }
}
