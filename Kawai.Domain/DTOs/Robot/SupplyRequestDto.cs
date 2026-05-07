namespace Kawai.Domain.DTOs.Robot;

public class SupplyRequestDto
{
    public string RequestSendID { get; set; }
    public string LineCode { get; set; }
    public string WorkStationCode { get; set; }
    public DateTime ProductionDate { get; set; }
    public string Model { get; set; }
    public string TrolleyCls { get; set; }
    public DateTime PickingTime { get; set; }
    public string StopPoint { get; set; }
    public int PickupSequence { get; set; }
    public int Status { get; set; }
}
