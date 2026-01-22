using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileIQCSample
{
    [Required]
    public long ReceiptId { get; set; }

    public long? InspectionId { get; set; }

    public string ReceiptNo { get; set; }

    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }

    [Required(ErrorMessage = "Qty Sample tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public decimal QtySample { get; set; }
}
