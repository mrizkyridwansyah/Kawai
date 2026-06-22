using Microsoft.AspNetCore.Http;

namespace Kawai.Domain.DTOs;

// INI DIPAKE DI ANDON WOMIN
public class AndonProductionDto: DataTableDto
{
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public string ModelCls { get; set; }
    public string ModelDescs { get; set; }
    public string LineStatus { get; set; }
    public string Production_Image { get; set; }
    public string Production_Lot { get; set; }
    public string Production_Name { get; set; }
    public string Production_Color { get; set; }
    public string Production_Progress { get; set; }
    public string Production_Target { get; set; }
    public string Production_Actual { get; set; }
    public string Production_Cycle { get; set; }
    public string Production_Remaining { get; set; }
    public string ScheduleDate { get; set; }
    public string Model { get; set; }
    public string PlanQty { get; set; }
    public string ResultQty { get; set; }
    public string Production_Status { get; set; }
    public string TotalSchedule { get; set; }
    public string AddressCode { get; set; }
    public string AreaCode { get; set; }
    public string Trolley_No { get; set; }
    public string Qty { get; set; }
    public string WorkStationCode { get; set; }
    public string WorkStationName { get; set; }
    public IFormFile ImageAttachment { get; set; }
    public string ImageName { get; set; }
    public byte[] ImageBase64 { get; set; }



}


