//using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace Kawai.Domain.DTOs.Mobile;

public class LoadingConfirmationInstructionDto
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
public class LoadingConfirmationListDto
{
    public string PartNo { get; set; }
    public string PartName { get; set; }
    public string SerialNo { get; set; }
    public string BarcodeNo { get; set; }

    [JsonIgnore]
    public DateTime? LoadingDate { get; set; }

    [JsonProperty("LoadingDate")]
    public string? LoadingDateDisplay => LoadingDate?.ToString("yyyy-MM-dd");

    [JsonIgnore]
    public TimeSpan? LoadingTime { get; set; }

    [JsonProperty("LoadingTime")]
    public string? LoadingTimeDisplay => LoadingTime?.ToString(@"hh\:mm\:ss");

    public bool IsScanned { get; set; }
    public string StatusLoading { get; set; }
    public int TotalPlan { get; set; }
    public int TotalScanned { get; set; }
    public bool IsAllScanned { get; set; }
    public bool BeforeStatus { get; set; }
    public bool AfterStatus { get; set; }
    public bool AllowEvidenceBefore { get; set; }
    public bool AllowEvidenceAfter { get; set; }
    public bool AllowScanBarcode { get; set; }
}

public class LoadingConfirmationDetailDto
{
    public string InstructionNo { get; set; }
    public string PartNo { get; set; }
    public string PartName { get; set; }
    public string SerialNo { get; set; }
    public string BarcodeNo { get; set; }

    [JsonIgnore]
    public DateTime? LoadingDate { get; set; }

    [JsonProperty("LoadingDate")]
    public string? LoadingDateDisplay => LoadingDate?.ToString("yyyy-MM-dd");

    [JsonIgnore]
    public TimeSpan? LoadingTime { get; set; }

    [JsonProperty("LoadingTime")]
    public string? LoadingTimeDisplay => LoadingTime?.ToString(@"hh\:mm\:ss");

    // =========================
    // TAMBAHAN PREVIEW DATA
    // =========================
    public string LoadingNo { get; set; }
    public string StatusReceipt { get; set; }
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string SINo { get; set; }
    public string POSeqNo { get; set; }
    public string PONo { get; set; }
    public double? Qty { get; set; }
}

public class LoadingConfirmationEvidenceAttachmentDto
{
    public int SeqNo { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }

    public string FileExtension { get; set; }

    public DateTime? RegisterDate { get; set; }

    public string RegisterBy { get; set; }
}

public class LoadingConfirmationEvidenceBeforeDetailDto
{
    public bool HasData { get; set; }

    public string EvidenceNo { get; set; }

    public string LoadingNo { get; set; }

    public string EvidenceType { get; set; }

    public string ContainerNo { get; set; }

    public string VehicleNo { get; set; }

    public string SealNo { get; set; }

    public string Remark { get; set; }

    public string DriverName { get; set; }

    public string TransportVendor { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public int RevisionNo { get; set; }

    public DateTime? EvidenceDate { get; set; }

    public DateTime? RegisterDate { get; set; }

    public string RegisterBy { get; set; }

    public List<LoadingConfirmationEvidenceAttachmentDto> Attachments { get; set; }
        = new();
}

public class LoadingConfirmationEvidenceAfterDetailDto
{
    public bool HasData { get; set; }

    public string EvidenceNo { get; set; }

    public string LoadingNo { get; set; }

    public string EvidenceType { get; set; }

    public string ContainerNo { get; set; }

    public string VehicleNo { get; set; }

    public string SealNo { get; set; }

    public string Remark { get; set; }

    public string DriverName { get; set; }

    public string TransportVendor { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public int RevisionNo { get; set; }

    public DateTime? EvidenceDate { get; set; }

    public DateTime? RegisterDate { get; set; }

    public string RegisterBy { get; set; }

    public List<LoadingConfirmationEvidenceAttachmentDto> Attachments { get; set; }
        = new();
}