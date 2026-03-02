using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileLoadingTrolley
{
    [Required(ErrorMessage = "Trolley No tidak boleh kosong")]
    public string TrolleyNo { get; set; }

    [Required(ErrorMessage = "Barcode No tidak boleh kosong")]
    public string BarcodeNo { get; set; }
}

public class MobileLoadingTrolleyComplete
{
    [Required(ErrorMessage = "Trolley No tidak boleh kosong")]
    public string TrolleyNo { get; set; }

    [Required(ErrorMessage = "Picking No tidak boleh kosong")]
    public string PickingNo { get; set; }
}
