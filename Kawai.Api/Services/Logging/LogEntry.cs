namespace Kawai.Api.Services.Logging;

/// <summary>
/// Model untuk buffer RequestLog sebelum di-flush ke database.
/// </summary>
public class RequestLogEntry
{
    public string Method { get; set; } = "";
    public string RequestPath { get; set; } = "";
    public string Token { get; set; } = "";
    public string RemoteAddr { get; set; } = "";
    public string UserID { get; set; } = "";
    public string FullName { get; set; } = "";
    public long Timestamp { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public string? RequestBody { get; set; }
}

/// <summary>
/// Model untuk buffer ErrorLog sebelum di-flush ke database.
/// </summary>
public class ErrorLogEntry
{
    public long Date { get; set; }
    public string? Message { get; set; }
    public string? Method { get; set; }
    public string? UserAgent { get; set; }
    public string? RemoteAddr { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestBody { get; set; }
    public string? StackTrace { get; set; }
    public int StatusCode { get; set; }
    public string? UserId { get; set; }
    public string? FullName { get; set; }
}

/// <summary>
/// Model untuk buffer MQLog sebelum di-flush ke database.
/// </summary>
public class MQLogEntry
{
    public long TimeStamp { get; set; }
    public string? TransactionType { get; set; }
    public string? FormatMessage { get; set; }
    public string? Method { get; set; }
    public string? RequestPath { get; set; }
    public string? UserID { get; set; }
    public string? FullName { get; set; }
    public long ElapsedMilliseconds { get; set; }
}
