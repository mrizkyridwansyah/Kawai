namespace Kawai.Domain.DTOs.Log;

public class LoggingRequestDto : DataTableDto
{
    public long Id { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string Token { get; set; }
    public string IP { get; set; }
    public string UserID { get; set; }
    public string FullName { get; set; }
    public long Timestamp { get; set; }
    public long ElapsedtimeMs { get; set; }
}
