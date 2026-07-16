using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class NGClaim
{
    public long? ClaimId { get; set; }

    public string ClaimNo { get; set; }
   
    [Required(ErrorMessage = "DN Number tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "DN Number tidak boleh lebih dari 50 karakter")]
    public string DNNumber { get; set; }

    
    [Required(ErrorMessage = "Supplier Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Supplier Code tidak boleh lebih dari 15 karakter")]
    public string SupplierCode { get; set; }

    [Required(ErrorMessage = "DN Date tidak boleh kosong")]
    public DateTime? DNDate { get; set; }


    
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

   
    [Required(ErrorMessage = "List Detail PO harus diisi")]
    public List<NGClaimDetail> Details { get; set; } = new List<NGClaimDetail>();
}

public class NGClaimDetail
{
   

    [Required(ErrorMessage = "Receipt Number tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "Receipt Number tidak boleh lebih dari 50 karakter")]
    public string ReceiptNumber { get; set; }

    [Required(ErrorMessage = "Item Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Item Code tidak boleh lebih dari 25 karakter")]
    public string ItemCode { get; set; }

    public decimal RemainingClaimQty { get; set; }
    public decimal ClaimReceiptQty { get; set; }




}
