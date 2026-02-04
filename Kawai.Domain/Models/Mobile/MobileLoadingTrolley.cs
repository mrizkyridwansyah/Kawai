using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileLoadingTrolley
{
    [Required(ErrorMessage = "Trolley No tidak boleh kosong")]
    public string TrolleyNo { get; set; }

    [Required(ErrorMessage = "Request Detail Id tidak boleh kosong")]
    public long? RequestDetailId { get; set; }
}
