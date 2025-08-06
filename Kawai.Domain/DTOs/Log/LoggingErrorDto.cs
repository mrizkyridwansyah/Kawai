namespace Kawai.Domain.DTOs.Log;

public class LoggingErrorDto : DataTableDto
{
    public long Id { get; set; }
    public long Date { get; set; }
    public string Message { get; set; }
    public string Level { get; set; }
    public string StackTrace { get; set; }
    public string InnerStackTrace { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string UserAgent { get; set; }
    public string RemoteAddr { get; set; }
    public int StatusCode { get; set; }
    public string Method { get; set; }
    public string RequestPath { get; set; }
    public string RequestBody { get; set; }
    public long ElapsedMilliseconds { get; set; }
}
