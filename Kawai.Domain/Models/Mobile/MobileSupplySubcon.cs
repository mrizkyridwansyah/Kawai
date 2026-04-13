using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileSupplySubcon
{
    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }

    public string LotNo { get; set; }

    public string ItemCode { get; set; }

    [Required(ErrorMessage = "Request No. boleh kosong")]
    public string RequestNoCode { get; set; }

    public string ItemClass { get; set; }

    [Required(ErrorMessage = "Qty tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public decimal Qty { get; set; }
}
