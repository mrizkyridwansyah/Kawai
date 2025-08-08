using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Item
{
    [Required(ErrorMessage = "Item Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Item Code tidak boleh lebih dari 25 karakter")]
    public string ItemCode { get; set; }

    [Required(ErrorMessage = "Item Name tidak boleh kosong")]
    [MaxLength(75, ErrorMessage = "Item Name tidak boleh lebih dari 75 karakter")]
    public string ItemName { get; set; }

    /// <summary>
    /// 01 = FINISH GOOD
    /// 02 = PARTS/WIP/MATERIAL
    /// </summary>
    [Required(ErrorMessage = "Finish Good Part Cls tidak boleh kosong")]
    [AllowedValues(["01", "02"])]    
    public string FinishGoodPartCls { get; set; }

    public string PartNumber { get; set; }
    public string DrawingCode { get; set; }

    [Required(ErrorMessage = "Warehouse Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Warehouse Code tidak boleh lebih dari 15 karakter")]
    public string WarehouseCode { get; set; }

    [MaxLength(15, ErrorMessage = "Address tidak boleh lebih dari 15 karakter")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Supplier Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Supplier Code tidak boleh lebih dari 15 karakter")]
    public string SupplierCode { get; set; }

    [MaxLength(15, ErrorMessage = "Delivery Code tidak boleh lebih dari 15 karakter")]
    public string DeliveryPlaceCode { get; set; }

    [MaxLength(15, ErrorMessage = "Manufacture Code tidak boleh lebih dari 15 karakter")]
    public string ManufactureCode { get; set; }

    [MaxLength(15, ErrorMessage = "Line Code tidak boleh lebih dari 15 karakter")]
    public string LineCode { get; set; }

    [Required(ErrorMessage = "Maker Item Code tidak boleh kosong")]
    [MaxLength(30, ErrorMessage = "Maker Item Code tidak boleh lebih dari 30 karakter")]
    public string MakerItemCode { get; set; }

    [Required(ErrorMessage = "Part Cls tidak boleh kosong")]
    [AllowedValues(["01", "02", "03", "04"])]
    public string PartCls { get; set; }

    public bool ReserveCls { get; set; }
    public bool SupplyCls { get; set; }
    public bool ProvisionCls { get; set; }
    public bool ProductionCls { get; set; }


    [MaxLength(2, ErrorMessage = "Material Cls tidak boleh lebih dari 2 karakter")]
    public string MaterialCls { get; set; }

    public double? Thickness { get; set; }
    public double? Width { get; set; }
    public double? Length { get; set; }
    public double? Weight { get; set; }
    public double? GrossWeight { get; set; }

    [MaxLength(2, ErrorMessage = "Sheet Coil Cls tidak boleh lebih dari 2 karakter")]
    public string SheetCoilCls { get; set; }
    public double? Pitch { get; set; }
    public double? Number_Producible { get; set; }
    public double? Scrap_Weight { get; set; }

    [MaxLength(2, ErrorMessage = "Drawing Material Cls tidak boleh lebih dari 2 karakter")]
    public string DrawingMaterialCls { get; set; }

    [MaxLength(2, ErrorMessage = "Surface Treatment Cls tidak boleh lebih dari 2 karakter")]
    public string SurfaceTreatmentCls { get; set; }
    public double? SurfaceOrderPointQty { get; set; }

    [MaxLength(2, ErrorMessage = "Heat Treatment Cls tidak boleh lebih dari 2 karakter")]
    public string HeatTreatmentCls { get; set; }
    public double? HeatOrderPointQty { get; set; }
    public double? Sample { get; set; }
    public double? SWQty { get; set; }
    public double? EWQty { get; set; }
    public double? NumberProcess { get; set; }
    public double? MaterialCoefficient { get; set; }
    public double? ProcessCoefficient { get; set; }
    public double? MinLot { get; set; }
    public double? LotQty { get; set; }
    public double? LotCoefficience { get; set; }
    public double? ProductReadTime { get; set; }
    public double? YieldPercentage { get; set; }
    public double? NumberEntering { get; set; }

    [MaxLength(2, ErrorMessage = "Packing Style Cls tidak boleh lebih dari 2 karakter")]
    public string PackingStyleCls { get; set; }

    [MaxLength(2, ErrorMessage = "Group Cls tidak boleh lebih dari 2 karakter")]
    public string GroupCls { get; set; }

    public double? StandardStock { get; set; }
    public double? SafetyStock { get; set; }
    public double? MaxStock { get; set; }
    public double? MinStock { get; set; }
    public double? AlowanceDay { get; set; }
    public double? DeliveryReadTime { get; set; }

    /// <summary>
    /// 01 = MAKE
    /// 02 = BUY
    /// </summary>
    [Required(ErrorMessage = "Make Or Buy Cls tidak boleh kosong")]
    [AllowedValues(["01", "02"])]
    public string MakeBuyCls { get; set; }

    [Required(ErrorMessage = "Control Cls tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Control Cls tidak boleh lebih dari 2 karakter")]
    public string ControlCls { get; set; }

    public double? OrderPointQty { get; set; }

    [MaxLength(2, ErrorMessage = "Unit Cls tidak boleh lebih dari 2 karakter")]
    public string UnitCls { get; set; }

    public double? NumberBox { get; set; }

    [MaxLength(2, ErrorMessage = "Packing Style Cls tidak boleh lebih dari 2 karakter")]
    public string PackingStyleMaterialCls { get; set; }

    public string AccountingCode { get; set; }

    [Required(ErrorMessage = "Explosion Cls tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Explosion Cls tidak boleh lebih dari 2 karakter")]
    public string ExplosionCls { get; set; }

    [MaxLength(2, ErrorMessage = "Person In Charge Cls tidak boleh lebih dari 2 karakter")]
    public string PersonInChargeCls { get; set; }

    [Required(ErrorMessage = "Stock Control Cls tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Stock Control Cls tidak boleh lebih dari 2 karakter")]
    public string StockControlCls { get; set; }

    [MaxLength(2, ErrorMessage = "Supply Issue Cls tidak boleh lebih dari 2 karakter")]
    public string SupplyIssueCls { get; set; }

    public DateTime? UseEndDay { get; set; }

    [MaxLength(15, ErrorMessage = "HS Code tidak boleh lebih dari 15 karakter")]
    public string HSCode { get; set; }

    public double? MinOrder { get; set; }
    public double? SafetyStockPercentage { get; set; }

    [MaxLength(18, ErrorMessage = "SAP Item Code tidak boleh lebih dari 18 karakter")]
    public string SAPItemCode { get; set; }

    [MaxLength(2, ErrorMessage = "Type Accs tidak boleh lebih dari 2 karakter")]
    public string TypeAccs { get; set; }

    [MaxLength(10, ErrorMessage = "Model Cls tidak boleh lebih dari 10 karakter")]
    public string ModelCls { get; set; }

    [MaxLength(2, ErrorMessage = "PO Type Cls tidak boleh lebih dari 2 karakter")]
    public string POTypeCls { get; set; }

    [MaxLength(2, ErrorMessage = "Classification Part Cls tidak boleh lebih dari 2 karakter")]
    public string ClasificationPartCls { get; set; }

    [MaxLength(2, ErrorMessage = "Destination Cls tidak boleh lebih dari 2 karakter")]
    public string DestinationCls { get; set; }

    [MaxLength(2, ErrorMessage = "Color Cls tidak boleh lebih dari 2 karakter")]
    public string ColorCls { get; set; }
}
