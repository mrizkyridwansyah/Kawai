using Kawai.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace Kawai.Api.Hub;

public class NotifApprovalHub : Microsoft.AspNetCore.SignalR.Hub
{
    private static readonly Dictionary<string, List<string>> _connections = new();

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        var token = httpContext.Request.Cookies["__SIDX"]
                     ?? httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        Console.WriteLine($"Client connected: {token}");

        if (!string.IsNullOrEmpty(token))
        {
            var sessionManager = SessionManagerAccessor.Instance;
            var result = await sessionManager?.ValidateToken(token)!;

            if (result?.IsSucceeded == true && !string.IsNullOrEmpty(result.Session?.UserId))
            {
                var userId = result.Session.UserId;

                lock (_connections)
                {
                    if (!_connections.ContainsKey(userId))
                        _connections[userId] = [];

                    _connections[userId].Add(Context.ConnectionId);

                    Console.WriteLine($"Client connected: {userId}");
                }
            }
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        lock (_connections)
        {
            foreach (var userId in _connections.Keys.ToList())
            {
                Console.WriteLine($"Client disconnected: {userId}");

                _connections[userId].Remove(Context.ConnectionId);
                if (_connections[userId].Count == 0)
                    _connections.Remove(userId);
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    public static IReadOnlyList<string> GetConnectionsForUser(string userId)
    {
        lock (_connections)
        {
            return _connections.TryGetValue(userId, out var connections) ? connections : [];
        }
    }

    //public async Task SendMessage(string user, string message)
    //{
    //    await Clients.All.SendAsync("ReceiveMessage", user, message);
    //}
}
