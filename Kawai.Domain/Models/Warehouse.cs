using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Warehouse
{
    [Required(ErrorMessage = "Factory Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Factory Code tidak boleh lebih dari 25 karakter")]
    public string FactoryCode { get; set; }

    [Required(ErrorMessage = "Warehouse Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Warehouse Code tidak boleh lebih dari 25 karakter")]
    public string WarehouseCode { get; set; }

    [Required(ErrorMessage = "Warehouse Name tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "Warehouse Name tidak boleh lebih dari 100 karakter")]
    public string WarehouseName { get; set; }

    [Required(ErrorMessage = "Adm Group tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Adm Group tidak boleh lebih dari 15 karakter")]
    public string AdmGroup { get; set; }

    [Required(ErrorMessage = "Flag Stock Control tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Flag Stock Control tidak boleh lebih dari 2 karakter")]
    [AllowedValues(["01", "02"])]
    public string StockControlCls { get; set; }

    [Required(ErrorMessage = "Flag NG tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Flag NG tidak boleh lebih dari 2 karakter")]
    [AllowedValues(["01", "02"])]
    public string NGCls { get; set; }

    public DateTime? UseEndDate { get; set; }
}
