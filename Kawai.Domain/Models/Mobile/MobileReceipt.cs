using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileReceipt
{
    [Required]
    public long Id { get; set; }

    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }

    [Required(ErrorMessage = "Qty tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public double Qty { get; set; }

    [Required(ErrorMessage = "Qty tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public double QtyVerify { get; set; }
}
