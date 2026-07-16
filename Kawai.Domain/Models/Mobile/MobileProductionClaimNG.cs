using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;
 

public class MobilProductionClaimNGSubmit
{
     
    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }
    public string WarehouseCode { get; set; }
    public string LineCode { get; set; }
    public string LotNo { get; set; }
    public string ItemCode { get; set; }
    public string ClaimNo { get; set; }
    public string PickingNo { get; set; }
    public decimal Qty { get; set; }
}




