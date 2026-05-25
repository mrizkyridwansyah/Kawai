using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileLoadingConfirmationSubmit
{
    [Required(ErrorMessage = "Shipping Instruction No tidak boleh kosong")]
    public string InstructionNo { get; set; }

    [Required(ErrorMessage = "Barcode No tidak boleh kosong")]
    public string BarcodeNo { get; set; }
}

public class MobileLoadingConfirmationEvidenceFile
{
    [Required(ErrorMessage = "File wajib diupload")]
    public IFormFile File { get; set; }

    public string Remarks { get; set; }
}

public class MobileLoadingConfirmationEvidenceBeforeSubmit
{
    [Required(ErrorMessage = "Instruction No tidak boleh kosong")]
    public string InstructionNo { get; set; }

    [Required(ErrorMessage = "Container No tidak boleh kosong")]
    public string ContainerNo { get; set; }

    [Required(ErrorMessage = "Vehicle No tidak boleh kosong")]
    public string VehicleNo { get; set; }

    [Required(ErrorMessage = "Seal No tidak boleh kosong")]
    public string SealNo { get; set; }

    public string Remark { get; set; }

    public string DriverName { get; set; }

    public string TransportVendor { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public string RevisionReason { get; set; }

    public List<MobileLoadingConfirmationEvidenceFile> Files { get; set; }

    public List<ExistingEvidenceFile> ExistingFiles { get; set; }
}

public class MobileLoadingConfirmationEvidenceAfterSubmit
{
    [Required(ErrorMessage = "Instruction No tidak boleh kosong")]
    public string InstructionNo { get; set; }

    [Required(ErrorMessage = "Container No tidak boleh kosong")]
    public string ContainerNo { get; set; }

    [Required(ErrorMessage = "Vehicle No tidak boleh kosong")]
    public string VehicleNo { get; set; }

    [Required(ErrorMessage = "Seal No tidak boleh kosong")]
    public string SealNo { get; set; }

    public string Remark { get; set; }

    public string DriverName { get; set; }

    public string TransportVendor { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public string RevisionReason { get; set; }

    public List<MobileLoadingConfirmationEvidenceFile> Files { get; set; }

    public List<ExistingEvidenceFile> ExistingFiles { get; set; }
}

public class MobileLoadingConfirmationEvidenceBeforeQueueSubmit
{
    public string InstructionNo { get; set; }

    public string ContainerNo { get; set; }

    public string VehicleNo { get; set; }

    public string SealNo { get; set; }

    public string Remark { get; set; }

    public string DriverName { get; set; }

    public string TransportVendor { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public string RevisionReason { get; set; }

    public List<LoadingConfirmationEvidenceFileQueue> Files { get; set; }

    public List<ExistingEvidenceFile> ExistingFiles { get; set; }
}

public class MobileLoadingConfirmationEvidenceAfterQueueSubmit
{
    public string InstructionNo { get; set; }

    public string ContainerNo { get; set; }

    public string VehicleNo { get; set; }

    public string SealNo { get; set; }

    public string Remark { get; set; }

    public string DriverName { get; set; }

    public string TransportVendor { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public string RevisionReason { get; set; }

    public List<LoadingConfirmationEvidenceFileQueue> Files { get; set; }

    public List<ExistingEvidenceFile> ExistingFiles { get; set; }
}

public class LoadingConfirmationEvidenceFileQueue
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string Remarks { get; set; }
}

public class ExistingEvidenceFile
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string Remarks { get; set; }
}