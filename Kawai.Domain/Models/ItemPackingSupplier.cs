using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ItemPackingSupplier
{
    [Required(ErrorMessage = "Supplier Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Supplier Code tidak boleh lebih dari 15 karakter")]
    public string SupplierCode { get; set; }

    public string PrevItemCode { get; set; }

    [Required(ErrorMessage = "Item Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Item Code tidak boleh lebih dari 25 karakter")]
    public string ItemCode { get; set; }

    [Required(ErrorMessage = "Item Code tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public double? QtyPacking { get; set; }
}
