using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobilePickingByScanSubmit
{
    [Required(ErrorMessage = "Shipping Instruction No tidak boleh kosong")]
    public string InstructionNo { get; set; }

    [Required(ErrorMessage = "Barcode No tidak boleh kosong")]
    public string BarcodeNo { get; set; }
}

public class MobilePickingByScanBarcodeScan
{
    [Required(ErrorMessage = "Shipping Instruction No tidak boleh kosong")]
    public string InstructionNo { get; set; }

    [Required(ErrorMessage = "Barcode No tidak boleh kosong")]
    public string BarcodeNo { get; set; }
    public string WHCode { get; set; }
}