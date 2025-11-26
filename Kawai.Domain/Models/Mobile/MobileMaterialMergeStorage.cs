using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileMaterialMergeStorage
{
    public string RefNo { get; set; }   

    [Required(ErrorMessage = "Address tidak boleh kosong")]
    public string AddressCode { get; set; }

    [Required(ErrorMessage = "List Barcode tidak boleh kosong")]
    public List<MobileMaterialMergeDetail> Details { get; set; }
}

public class MobileMaterialMergeDetail
{
    public string ItemCode { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
}
