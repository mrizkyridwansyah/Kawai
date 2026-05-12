using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ReceiptUnschedule
{
    public long? Id { get; set; }

    public string ReceiptNo { get; set; }

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

    [Required(ErrorMessage = "BC Number tidak boleh kosong")]
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

    [Required(ErrorMessage = "Reference No tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "Reference No tidak boleh lebih dari 50 karakter")]
    public string ReferenceNo { get; set; }

    [Required(ErrorMessage = "List Detail Item harus diisi")]
    public List<ReceiptUnscheduleDetail> Details { get; set; } = new List<ReceiptUnscheduleDetail>();
}

public class ReceiptUnscheduleDetail
{
    [Required(ErrorMessage = "Item Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Item Code tidak boleh lebih dari 25 karakter")]
    public string ItemCode { get; set; }

    [Required(ErrorMessage = "Receipt Qty tidak boleh kosong")]
    [NumberGreaterThan(0)]
    public decimal ReceiptQty { get; set; }
}
