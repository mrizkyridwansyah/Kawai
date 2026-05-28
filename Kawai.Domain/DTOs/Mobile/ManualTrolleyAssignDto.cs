namespace Kawai.Domain.DTOs.Mobile;

public class ManualTrolleyAssignDto
{
    public string RequestNo { get; set; }
    public string TrolleyNo { get; set; }
    public bool IsCurrentProcessManual { get; set; }
    public string LastStopPointComplete { get; set; }
}


public class ManualTrolleyAssignValidation
{
    public bool AlreadyHadStock { get; set; }
    public string MessageConfirmation { get; set; }
}
