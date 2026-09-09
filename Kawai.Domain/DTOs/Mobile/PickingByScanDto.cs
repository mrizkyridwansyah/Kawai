//using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace Kawai.Domain.DTOs.Mobile;

public class PickingByScanInstructionDto
{
    public string InstructionNo { get; set; }
    public string Customer { get; set; }

    [JsonIgnore]
    public DateTime InstructionDate { get; set; }

    [JsonProperty("InstructionDate")]
    public string InstructionDateDisplay => InstructionDate.ToString("yyyy-MM-dd HH:mm:ss");
}

// ==============================
// GRID LIST
// ==============================
public class PickingByScanListDto
{
    public string PartNo { get; set; }
    public string PartName { get; set; }
    public string SerialNo { get; set; }
    public string BarcodeNo { get; set; }

    [JsonIgnore]
    public DateTime? PickingDate { get; set; }

    [JsonProperty("PickingDate")]
    public string? PickingDateDisplay => PickingDate?.ToString("yyyy-MM-dd");

    [JsonIgnore]
    public TimeSpan? PickingTime { get; set; }

    [JsonProperty("PickingTime")]
    public string? PickingTimeDisplay => PickingTime?.ToString(@"hh\:mm\:ss");

    public bool IsScanned { get; set; }
    public string StatusPicking { get; set; }
    public decimal QtyShipping { get; set; }
    public decimal QtyPicking { get; set; }
}

public class PickingByScanDetailDto
{
    public string InstructionNo { get; set; }
    public string PartNo { get; set; }
    public string PartName { get; set; }
    public string SerialNo { get; set; }
    public string BarcodeNo { get; set; }

    [JsonIgnore]
    public DateTime? PickingDate { get; set; }

    [JsonProperty("PickingDate")]
    public string? PickingDateDisplay => PickingDate?.ToString("yyyy-MM-dd");

    [JsonIgnore]
    public TimeSpan? PickingTime { get; set; }

    [JsonProperty("PickingTime")]
    public string? PickingTimeDisplay => PickingTime?.ToString(@"hh\:mm\:ss");

    // =========================
    // TAMBAHAN PREVIEW DATA
    // =========================
    public string PickingNo { get; set; }
    public string StatusReceipt { get; set; }
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string SINo { get; set; }
    public string POSeqNo { get; set; }
    public string PONo { get; set; }
    public double? Qty { get; set; }
}

//public class ApiResponseDto
//{
//    public bool Success { get; set; }
//    public string Message { get; set; }
//    public object Data { get; set; }
//}