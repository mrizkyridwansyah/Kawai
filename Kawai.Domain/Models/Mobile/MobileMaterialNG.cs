using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileMaterialNG
{
    public long? InspectionId { get; set; }

    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }

    [Required(ErrorMessage = "Qty NG tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public decimal QtyNG { get; set; }
}
