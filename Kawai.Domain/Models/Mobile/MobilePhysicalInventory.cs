using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobilePhysicalInventory
{
    [Required(ErrorMessage = "Address tidak boleh kosong")]
    public string AddressCode { get; set; }

    [Required(ErrorMessage = "Barcode tidak boleh kosong")]
    public string BarcodeNo { get; set; }

    [Required(ErrorMessage = "Inventory tidak boleh kosong")]    
    public double? InventoryQty { get; set; }
}
