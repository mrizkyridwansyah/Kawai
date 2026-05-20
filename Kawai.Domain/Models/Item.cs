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

    [MaxLength(100, ErrorMessage = "Drawing Number tidak boleh lebih dari 100 karakter")]
    public string DrawingNumber{ get; set; }

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

    [Required(ErrorMessage = "Reserver Cls tidak boleh kosong")]
    public string ReserveCls { get; set; }

    [Required(ErrorMessage = "Supply Cls tidak boleh kosong")]
    public string SupplyCls { get; set; }

    [Required(ErrorMessage = "Provision Cls tidak boleh kosong")]
    public string ProvisionCls { get; set; }

    [Required(ErrorMessage = "Production Cls tidak boleh kosong")]
    public string ProductionCls { get; set; }

    [Required(ErrorMessage = "Stock Control Cls tidak boleh kosong")]
    public string StockControlCls { get; set; }


    [MaxLength(2, ErrorMessage = "Material Cls tidak boleh lebih dari 2 karakter")]
    public string MaterialCls { get; set; }

    public decimal? Thickness { get; set; }
    public decimal? Width { get; set; }
    public decimal? Length { get; set; }
    public decimal? Weight { get; set; }
    public decimal? GrossWeight { get; set; }

    [MaxLength(2, ErrorMessage = "Sheet Coil Cls tidak boleh lebih dari 2 karakter")]
    public string SheetCoilCls { get; set; }
    public decimal? Pitch { get; set; }
    public decimal? NumberProducible { get; set; }
    public decimal? ScrapWeight { get; set; }

    [MaxLength(2, ErrorMessage = "Drawing Material Cls tidak boleh lebih dari 2 karakter")]
    public string DrawingMaterialCls { get; set; }

    [MaxLength(2, ErrorMessage = "Surface Treatment Cls tidak boleh lebih dari 2 karakter")]
    public string SurfaceTreatmentCls { get; set; }
    public decimal? SurfaceOrderPointQty { get; set; }

    [MaxLength(2, ErrorMessage = "Heat Treatment Cls tidak boleh lebih dari 2 karakter")]
    public string HeatTreatmentCls { get; set; }
    public decimal? HeatOrderPointQty { get; set; }
    public decimal? Sample { get; set; }
    public decimal? SWQty { get; set; }
    public decimal? EWQty { get; set; }
    public decimal? NumberProcess { get; set; }
    public decimal? MaterialCoefficient { get; set; }
    public decimal? ProcessCoefficient { get; set; }
    public decimal? MinLot { get; set; }
    public decimal? LotQty { get; set; }
    public decimal? LotCoefficience { get; set; }
    public decimal? ProductReadTime { get; set; }
    public decimal? YieldPercentage { get; set; }
    public decimal? NumberEntering { get; set; }

    [MaxLength(2, ErrorMessage = "Packing Style Cls tidak boleh lebih dari 2 karakter")]
    public string PackingStyleCls { get; set; }

    [MaxLength(2, ErrorMessage = "Group Cls tidak boleh lebih dari 2 karakter")]
    public string GroupCls { get; set; }

    public decimal? StandardStock { get; set; }
    public decimal? SafetyStock { get; set; }
    public decimal? MaxStock { get; set; }
    public decimal? MinStock { get; set; }
    public decimal? AlowanceDay { get; set; }
    public decimal? DeliveryReadTime { get; set; }

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

    public decimal? OrderPointQty { get; set; }

    [MaxLength(2, ErrorMessage = "Unit Cls tidak boleh lebih dari 2 karakter")]
    public string UnitCls { get; set; }

    public decimal? NumberBox { get; set; }

    [MaxLength(2, ErrorMessage = "Packing Style Cls tidak boleh lebih dari 2 karakter")]
    public string PackingStyleMaterialCls { get; set; }

    [MaxLength(7, ErrorMessage = "Accounting Code tidak boleh lebih dari 7 karakter")]
    public string AccountingCode { get; set; }

    [Required(ErrorMessage = "Explosion Cls tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Explosion Cls tidak boleh lebih dari 2 karakter")]
    public string ExplosionCls { get; set; }

    [MaxLength(2, ErrorMessage = "Person In Charge Cls tidak boleh lebih dari 2 karakter")]
    public string PersonInChargeCls { get; set; }

    [MaxLength(2, ErrorMessage = "Supply Issue Cls tidak boleh lebih dari 2 karakter")]
    public string SupplyIssueCls { get; set; }

    public DateTime? UseEndDay { get; set; }

    [MaxLength(15, ErrorMessage = "HS Code tidak boleh lebih dari 15 karakter")]
    public string HSCode { get; set; }

    public decimal? MinOrder { get; set; }
    public decimal? SafetyStockPercentage { get; set; }

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
