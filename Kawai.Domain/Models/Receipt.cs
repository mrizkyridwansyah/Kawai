using Kawai.Domain.Shared;
using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kawai.Domain.Models;

public class Receipt
{
    public long? Id { get; set; }

    public string ReceiptNo { get; set; }
    public bool IsManual { get; set; } = true;

    [Required(ErrorMessage = "Receipt Date tidak boleh kosong")]
    public DateTime? ReceiptDate { get; set; }

    [Required(ErrorMessage = "DN Number tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "DN Number tidak boleh lebih dari 50 karakter")]
    public string DNNumber { get; set; }

    [Required(ErrorMessage = "Factory Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Factory Code tidak boleh lebih dari 15 karakter")]
    public string FactoryCode { get; set; }

    [Required(ErrorMessage = "Supplier Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Supplier Code tidak boleh lebih dari 15 karakter")]
    public string SupplierCode { get; set; }

    [Required(ErrorMessage = "DN Date tidak boleh kosong")]
    public DateTime? DNDate { get; set; }

    [RequiredIfEqual(nameof(BCType), "BC 2.7", ErrorMessage = "BC Number tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "BC Number tidak boleh lebih dari 50 karakter")]
    public string BCNumber { get; set; }

    [Required(ErrorMessage = "BC Type tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "BC Type tidak boleh lebih dari 15 karakter")]
    public string BCType { get; set; }

    [Required(ErrorMessage = "BC Date tidak boleh kosong")]
    public DateTime? BCDate { get; set; }

    [Required(ErrorMessage = "Vehicle No tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Vehicle No tidak boleh lebih dari 15 karakter")]
    public string VehicleNo { get; set; }

    [Required(ErrorMessage = "Transport tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Transport tidak boleh lebih dari 15 karakter")]
    public string Transport { get; set; }
    public string Remarks { get; set; }
    public string RegisterNo { get; set; }

    [Required(ErrorMessage = "List Detail PO harus diisi")]
    public List<ReceiptDetail> Details { get; set; } = new List<ReceiptDetail>();
}

public class ReceiptDetail
{
    [Required(ErrorMessage = "PO Number tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "PO Number tidak boleh lebih dari 50 karakter")]
    public string PONumber { get; set; }

    [Required(ErrorMessage = "Item Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Item Code tidak boleh lebih dari 25 karakter")]
    public string ItemCode { get; set; }

    public string UnitClsCode { get; set; }
    public decimal ExpectedQty { get; set; }
    public decimal TotalPacking { get; set; }

    [Required(ErrorMessage = "Receipt Qty tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public decimal ReceiptQty { get; set; }

    [Required(ErrorMessage = "No. Seri tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public int? NoSeri { get; set; }
}

public class ReceiptBreakdown
{
    [Required]
    public long? ReceiptId { get; set; }

    [Required]
    public long? ReceiptDetailId { get; set; }

    [Required]
    public List<ReceiptBreakdownDetail> BreakdownDetails { get; set; }
}

public class ReceiptBreakdownDetail
{
    [Required]
    [NumberGreaterThan(0)]
    public decimal? ReceiptQty { get; set; }

    [Required]
    [NumberGreaterThan(0)]
    public int? NoSeri { get; set; }
}

public class ReceiptImport
{
    public ReceiptHeaderImport Header { get; set; }
    public List<ReceiptDetailImport> Details { get; set; } = new();
}

public class ReceiptHeaderImport
{
      public string SupplierCode { get; set; }
      public DateTime? ReceiptDate { get; set; }
      public string BCType { get; set; }
      public DateTime? BCDate { get; set; }
      public string DNNumber { get; set; }
      public string BCNumber { get; set; }
      public string Errors { get; set; } = "";
}

public class ReceiptDetailImport : ImportBase
{

    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public decimal ReceiptQty { get; set; }
    

}