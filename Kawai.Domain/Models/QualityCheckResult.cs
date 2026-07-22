using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class QualityCheckResult
{
    [Required]
    public long InspectionId { get; set; }

    [Required(ErrorMessage = "Qty NG tidak boleh kosong")]
    public decimal QtyNG { get; set; }

    [Required(ErrorMessage = "Remarks tidak boleh kosong")]
    public string Remarks { get; set; }

    public long? AttachmentID{ get; set; }
    public string AttachmentName { get; set; }

    [RequiredIfNotNullOrZero(nameof(QtyNG), ErrorMessage = "Attachment tidak boleh kosong")]
    public IFormFile? Attachment { get; set; }

}


public class QualityCheckConfirm
{
    [Required]
    public long InspectionId { get; set; }

    [Required]
    [AllowedValues(["Accepted", "Rejected", "SA"])]
    public string InspectionResult { get; set; }

    [RequiredIfEqual(nameof(InspectionResult), "Rejected")]
    [AllowedValues(["Process", "Vendor"])]
    public string TypeHold { get; set; }

}

public class QualityCheckConfirmSA
{
    [Required]
    public long InspectionId { get; set; }

    [Required]
    [AllowedValues(["Accepted", "Rejected"])]
    public string InspectionResult { get; set; }

    [Required]
    public string RemarksSA { get; set; }

    [Required]
    public decimal? TotalGoodQty { get; set; }

    [Required]
    public bool? ProcessUnapprove { get; set; }

    [RequiredIfEqual(nameof(InspectionResult), "Rejected")]
    [AllowedValues(["Process", "Vendor"])]
    public string TypeHold { get; set; }

}
