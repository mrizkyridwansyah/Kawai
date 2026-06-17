using System.Diagnostics;
using System.Text;
using Kawai.Api.Services.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

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
        var auth = context.RequestServices.GetRequiredService<Auth>();
        var requestPath = context.Request.Path.Value + context.Request.QueryString.Value;
        string token = context.Request.Headers["Authorization"].ToString();
        string remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        long timeStamp = EpochDateTime.Now;
        string userId = auth?.User?.UserID ?? "";
        string fullname = auth?.User?.FullName ?? "";

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
        logBuffer.EnqueueRequestLog(new RequestLogEntry
        {
            Method = method,
            RequestPath = requestPath,
            Token = token,
            RemoteAddr = remoteIp,
            UserID = userId,
            FullName = fullname,
            Timestamp = timeStamp,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            RequestBody = requestBody
        });
    }

    /// <summary>
    /// Baca request body. Untuk multipart/form-data (upload file), hanya simpan metadata.
    /// </summary>
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

    /// <summary>
    /// Untuk multipart/form-data: hanya simpan metadata (nama field, nama file, ukuran),
    /// TIDAK menyimpan isi file.
    /// </summary>
    private static async Task<string?> ReadMultipartMetadata(HttpRequest request)
    {
        try
        {
            var boundary = HeaderUtilities.RemoveQuotes(
                MediaTypeHeaderValue.Parse(request.ContentType).Boundary
            ).Value;

            if (string.IsNullOrEmpty(boundary))
                return "[multipart: no boundary]";

            request.Body.Position = 0;
            var reader = new MultipartReader(boundary, request.Body);
            MultipartSection? section;

            var fields = new Dictionary<string, string>();

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var disposition = section.GetContentDispositionHeader();
                if (disposition == null) continue;

                if (disposition.IsFileDisposition())
                {
                    var key = disposition.Name.Value ?? "file";
                    var fileName = disposition.FileName.Value ?? "unknown";

                    // Hitung ukuran file tanpa menyimpan isi-nya
                    long size = 0;
                    var buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = await section.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        size += bytesRead;
                    }

                    fields[$"{key}_FileName"] = fileName;
                    fields[$"{key}_Size"] = FormatSize(size);
                }
                else if (disposition.IsFormDisposition())
                {
                    var key = disposition.Name.Value ?? "field";
                    using var streamReader = new StreamReader(section.Body, Encoding.UTF8);
                    var value = await streamReader.ReadToEndAsync();
                    fields[key] = value;
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
}
