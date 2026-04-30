using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileAssignToTrolley
{
    [Required(ErrorMessage = "Barcode No tidak boleh kosong")]
    public string BarcodeNo { get; set; }

    [Required(ErrorMessage = "Trolley No tidak boleh kosong")]
    public string TrolleyNo { get; set; }
}
