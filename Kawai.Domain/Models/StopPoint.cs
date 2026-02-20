using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class StopPoint
{
    [Required(ErrorMessage = "Stop Point Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Stop Point Code tidak boleh lebih dari 25 karakter")]
    public string StopPointCode { get; set; }

    [Required(ErrorMessage = "Description tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "Description tidak boleh lebih dari 100 karakter")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Picking Seq tidak boleh kosong")]
     public decimal? PickingSeq { get; set; }

    [Required(ErrorMessage = "Flag Active tidak boleh kosong")]
    public bool? IsActive { get; set; }
}

public class StopPointSetting
{
    public string StopPoint { get; set; }
    public List<AddressListSetting> Address { get; set; }
}

public class AddressListSetting
{
    public string Key { get; set; }

}
