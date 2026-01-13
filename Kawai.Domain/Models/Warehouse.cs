using Kawai.Domain.Shared;
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

[Importable]
public class WarehouseImport : ImportBase
{
    [Required(ErrorMessage = "Factory Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Factory Code tidak boleh lebih dari 25 karakter")]
    [ReferenceSheet("sp_Wms_Import_FactoryReference")]
    [Display(Name = "Factory Code")]
    public string FactoryCode { get; set; }

    [Required(ErrorMessage = "Warehouse Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Warehouse Code tidak boleh lebih dari 25 karakter")]
    [Display(Name = "Warehouse Code")]
    public string WarehouseCode { get; set; }

    [Required(ErrorMessage = "Warehouse Name tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "Warehouse Name tidak boleh lebih dari 100 karakter")]
    [Display(Name = "Warehouse Name")]
    public string WarehouseName { get; set; }

    [Required(ErrorMessage = "Adm Group tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Adm Group tidak boleh lebih dari 15 karakter")]
    [ReferenceSheet("sp_Wms_Import_AdmGroupReference")]
    [Display(Name = "Adm Group")]
    public string AdmGroup { get; set; }

    [Required(ErrorMessage = "Flag Stock Control tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Flag Stock Control tidak boleh lebih dari 2 karakter")]
    [ReferenceSheet("sp_Wms_Import_StockControlClsReference")]
    [AllowedValues(["01", "02"])]
    [Display(Name = "Stock Control Cls")]
    public string StockControlCls { get; set; }

    [Required(ErrorMessage = "Flag NG tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Flag NG tidak boleh lebih dari 2 karakter")]
    [ReferenceSheet("sp_Wms_Import_NGClsReference")]
    [AllowedValues(["01", "02"])]
    [Display(Name = "NG Cls")]
    public string NGCls { get; set; }

    [Display(Name = "Use End Date")]
    public DateTime? UseEndDate { get; set; }
}
