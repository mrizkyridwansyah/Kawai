using System.Diagnostics;
using Kawai.Data.SqlConnections;
using System.Data;
using System.Text;
using Kawai.Api.Robot.Services.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Kawai.Domain.Shared;

namespace Kawai.Api.Robot.Shared.Middleware;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, LogBufferService logBuffer)
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
        var requestPath = context.Request.Path.Value + context.Request.QueryString.Value;
        string token = context.Request.Headers["Authorization"].ToString();
        string userId = DecodeBasicAuth(token) ?? "";

        string remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        long timeStamp = EpochDateTime.Now;

        context.Request.EnableBuffering();

        // Baca request body sebelum pipeline (supaya body masih bisa dibaca controller)
        string? requestBody = null;
        try
        {
            requestBody = await ReadRequestBody(context.Request);
        }
        catch
        {
            requestBody = "[Failed to read request body]";
        }

        // Continue pipeline
        await _next(context);

        stopwatch.Stop();

        // Fire-and-forget: enqueue ke buffer, TIDAK await INSERT ke DB
        logBuffer.EnqueueRequestLog(new RequestAMRLogEntry
        {
            Method = method,
            RequestPath = requestPath,
            Token = token,
            RemoteAddr = remoteIp,
            UserID = remoteIp, // Berdasarkan kode lama: UserID = remoteIp
            FullName = userId, // Berdasarkan kode lama: FullName = userId
            Timestamp = timeStamp,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            RequestBody = requestBody
        });
    }

    private static async Task<string?> ReadRequestBody(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0)
            return null;

        if (request.ContentType?.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) == true)
        {
            return await ReadMultipartMetadata(request);
        }

        // JSON / form-urlencoded / plain text
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private static async Task<string?> ReadMultipartMetadata(HttpRequest request)
    {
        try
        {
            var boundary = HeaderUtilities.RemoveQuotes(
                MediaTypeHeaderValue.Parse(request.ContentType).Boundary
            ).Value;

            if (string.IsNullOrEmpty(boundary))
                return "[multipart: boundary not found]";

            var reader = new MultipartReader(boundary, request.Body);
            MultipartSection? section;
            var fields = new Dictionary<string, string>();

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDisposition = section.GetContentDispositionHeader();

                if (contentDisposition != null && contentDisposition.IsFileDisposition())
                {
                    fields[contentDisposition.Name.Value ?? "unknown"] = $"[FILE: {contentDisposition.FileName.Value}] - {FormatSize(section.Body.Length)}";
                }
                else if (contentDisposition != null && contentDisposition.IsFormDisposition())
                {
                    using var streamReader = new StreamReader(section.Body, Encoding.UTF8);
                    fields[contentDisposition.Name.Value ?? "unknown"] = await streamReader.ReadToEndAsync();
                }
            }

            request.Body.Position = 0;
            return System.Text.Json.JsonSerializer.Serialize(fields);
        }
        catch
        {
            return "[multipart: failed to parse]";
        }
    }

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }

    public string? DecodeBasicAuth(string authorizationHeader)
    {
        if (string.IsNullOrEmpty(authorizationHeader))
            return null;

        if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            return null;

        var base64 = authorizationHeader.Substring("Basic ".Length).Trim();

        var bytes = Convert.FromBase64String(base64);
        var decoded = Encoding.UTF8.GetString(bytes);

        var parts = decoded.Split(':', 2);

        if (parts.Length != 2)
            return null;

        return parts[0];
    }
}
