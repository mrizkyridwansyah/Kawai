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
 
    public string WarehouseCode { get; set; }


    public string LineCode { get; set; }
    public string LotNo { get; set; }
    public string ItemCode { get; set; }
    public string RequestNo { get; set; }
    public decimal Qty { get; set; }
}

