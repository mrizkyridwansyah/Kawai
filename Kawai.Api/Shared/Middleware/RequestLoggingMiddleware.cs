using System.Diagnostics;
using Kawai.Data.SqlConnections;
using System.Data;

namespace Kawai.Api.Shared.Middleware;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, LogExecutor logExecutor)
    {
        // Hanya log request yang ke endpoint /api
        if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        string[] allowedMethod = ["GET", "POST", "PATCH", "DELETE", "PUT"];
        string method = context.Request.Method;

        if (!allowedMethod.Contains(method))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var auth = context.RequestServices.GetRequiredService<Auth>();
        string requestPath = context.Request.Path;
        string token = context.Request.Headers["Authorization"].ToString();
        string remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        long timeStamp = EpochDateTime.Now;
        string userId = auth?.User?.UserID ?? "";
        string fullname = auth?.User?.FullName ?? "";

        context.Request.EnableBuffering();

        // Continue pipeline
        await _next(context);

        stopwatch.Stop();

        try
        {
            await logExecutor.ExecuteAsync(@"
                INSERT INTO [dbo].[RequestLogs]
                ([Method], [Path], [Token], [IP], [UserID], [FullName], [Timestamp], [ElapsedtimeMs])
                VALUES
                (@Method, @RequestPath, @Token, @RemoteAddr, @UserID, @FullName, @Date, @ElapsedMilliseconds)", new
            {
                Method = method,
                RequestPath = requestPath,
                Token = token,
                RemoteAddr = remoteIp,
                UserID = userId,
                FullName = fullname,
                Date = timeStamp,
                stopwatch.ElapsedMilliseconds
            }, commandType: CommandType.Text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write request log");
        }
    }
}

