using Kawai.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace Kawai.Api.Hub;

public class NotifApprovalHub : Microsoft.AspNetCore.SignalR.Hub
{
    private static readonly Dictionary<string, List<string>> _connections = new();

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var configuration = httpContext.RequestServices.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
        var cookieName = configuration?["Security:AuthCookieName"] ?? "__SIDX";

        var tokenCookie = httpContext.Request.Cookies[cookieName];

        var tokenAuth = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var tokenMobile = httpContext.Request.Query["Access_Token_Mobile"];

        var token = !String.IsNullOrEmpty(tokenCookie) ? tokenCookie : !String.IsNullOrEmpty(tokenAuth) ? tokenAuth : !String.IsNullOrEmpty(tokenMobile) ? tokenMobile : "";
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

                    if (!String.IsNullOrEmpty(token))
                    {
                        if (!_connections.ContainsKey(token))
                            _connections[token] = [];

                        _connections[token].Add(Context.ConnectionId);
                    }

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
