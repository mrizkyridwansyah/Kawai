using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class InventoryReport
{
    public long? Id { get; set; }

    public bool IsManual { get; set; }

    [Required(ErrorMessage = "Warehouse tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Warehouse tidak boleh lebih dari 15 karakter")]
    public string Warehouse { get; set; } = string.Empty;

    [Required(ErrorMessage = "Factory Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Factory Code tidak boleh lebih dari 15 karakter")]
    public string FactoryCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Period tidak boleh kosong")]
    public DateTime Period { get; set; }
}

