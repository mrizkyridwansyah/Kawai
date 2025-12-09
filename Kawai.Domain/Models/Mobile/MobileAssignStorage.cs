using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileAssignStorage
{
    [Required(ErrorMessage = "Ref No tidak boleh kosong")]
    public string RefNo { get; set; }

    [Required(ErrorMessage = "Address tidak boleh kosong")]
    public string AddressCode { get; set; }
}
