using Kawai.Api.Hub;
using Microsoft.AspNetCore.SignalR;

namespace Kawai.Api.Services;

public class NotificationService<THub> where THub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IHubContext<THub> _hubContext;

    public NotificationService(IHubContext<THub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadCastOnlyTo(List<string> receivers, string pesan, object data)
    {
        foreach (var userId in receivers)
        {
            var connections = GetConnectionsForUser(userId);
            foreach (var connectionId in connections)
            {
                await _hubContext.Clients.Client(connectionId).SendAsync(pesan, data);
            }
        }
    }

    public async Task BroadCastAll(string pesan, object data)
    {
        await _hubContext.Clients.All.SendAsync(pesan, data);
    }

    private IReadOnlyList<string> GetConnectionsForUser(string userId)
    {
        if (typeof(THub) == typeof(NotifApprovalHub))
        {
            return NotifApprovalHub.GetConnectionsForUser(userId);
        }
        // kalau ada hub lain, tambahkan else if atau buat interface supaya lebih rapi
        return new List<string>();
    }
}
