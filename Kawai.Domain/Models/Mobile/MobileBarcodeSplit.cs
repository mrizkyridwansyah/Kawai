using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Mobile;

public class MobileBarcodeSplit
{
    [Required]
    public string BarcodeNo { get; set; }

    public string BarcodeNoNew { get; set; }

    [Required]
    [NumberGreaterThan(0)]
    public decimal? QtySplit { get; set; }
}
