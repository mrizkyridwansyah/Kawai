namespace Kawai.Domain.Models;

public class StockTransactionMessage<T>
{
    public string AuthUserId { get; set; }
    public long TimeStamp { get; set; }
    public string TransactionType { get; set; }
    public string FormatMessage { get; set; }   
    public T Payload { get; set; }
}
