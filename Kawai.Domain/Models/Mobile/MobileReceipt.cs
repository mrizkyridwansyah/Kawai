using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileReceipt
{
    public string RefNo { get; set; }

    [Required]
    public List<MobileReceiptDetail> Details { get; set; } = new List<MobileReceiptDetail>();
}

public class MobileReceiptDetail
{
    [Required]
    public long ReceiptDetailBarcodeId { get; set; }

    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }

    [Required(ErrorMessage = "Qty tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public decimal Qty { get; set; }

    [Required(ErrorMessage = "Qty tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public decimal QtyVerify { get; set; }
}
