namespace Kawai.Domain.Models;

public class StockTransactionMessage<T>
{
    public string AuthUserId { get; set; }
    public long TimeStamp { get; set; }
    public string TransactionType { get; set; }
    public string FormatMessage { get; set; }   
    public T Payload { get; set; }
    public LogContext LogContext { get; set; }
}

public class LogContext
{
    public string Method { get; set; }
    public string RequestPath { get; set; }
    public string RemoteAddr { get; set; }
    public string UserAgent { get; set; }
    public string UserID { get; set; }
    public string FullName { get; set; }

}
