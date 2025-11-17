using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileMaterialStorage
{
    [Required(ErrorMessage = "Ref No tidak boleh kosong")]
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }

    [Required(ErrorMessage = "Address tidak boleh kosong")]
    public string AddressCode { get; set; }

    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }
}
