using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;
public class MobilSupplyScanRequest
{
    [Required(ErrorMessage = "Ref No tidak boleh kosong")]
    public string RefNo { get; set; }

    [Required(ErrorMessage = "Address tidak boleh kosong")]
    public string AddressCode { get; set; }

    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }
}

