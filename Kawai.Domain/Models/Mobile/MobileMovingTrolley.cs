using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileMovingTrolley
{
    [Required(ErrorMessage = "Request No harus diisi.")]
    public string RequestNo { get; set; }

    [Required(ErrorMessage = "Trolley No harus diisi.")]
    public string TrolleyNo { get; set; }

    [Required(ErrorMessage = "Stop Point harus diisi.")]
    public string StopPoint { get; set; }
}
