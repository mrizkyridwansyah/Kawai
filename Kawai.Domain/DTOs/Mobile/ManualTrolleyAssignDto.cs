namespace Kawai.Domain.DTOs.Mobile;

public class ManualTrolleyAssignDto
{
    public string RequestNo { get; set; }
    public string TrolleyNo { get; set; }
    public string TrolleyNoDesc { get; set; }
    public bool IsCurrentProcessManual { get; set; }
    public string LastStopPointComplete { get; set; }
}


public class ManualTrolleyAssignValidationDto
{
    public bool AlreadyHadStock { get; set; }
    public string MessageConfirmation { get; set; }
}

public class ManualTrolleyDetailRequestDto
{
    public string RequestNo { get; set; }
    public string TrolleyNo { get; set; }
    public bool IsCurrentProcessManual { get; set; }

    public string StopPoint { get; set; }
    public string StopPointDesc { get; set; }
    public string StatusAMR { get; set; }
    public bool IsCompleteLoading { get; set; }
    public string LastUserRequestAMR { get; set; }
    public DateTime? LastRequestDateAMR { get; set; }
}