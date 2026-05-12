using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileManualTrolleyAssign
{
    [Required(ErrorMessage = "Trolley No harus diisi.")]
    public string TrolleyNo { get; set; }

    [Required(ErrorMessage = "Request No harus diisi.")]
    public string RequestNo { get; set; }
}
