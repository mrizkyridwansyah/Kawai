namespace Kawai.Domain.Models.Robot;

public class CompleteStatusRequest
{        
    public string RequestSendID { get; set; }
    public string TrolleyNo { get; set; }
    public string StopPoint { get; set; }
    public int CompleteStatus { get; set; }
    public bool IsManual { get; set; }
}
