namespace Kawai.Domain.DTOs.Log;

public class LoggingMQDto: DataTableDto
{
    public long Id { get; set; }
    public long TimeStamp { get; set; }
    public string TransactionType { get; set; }
    public string FormatMessage { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string UserID { get; set; }
    public string FullName { get; set; }
    public long ElapsedtimeMs { get; set; }

}
