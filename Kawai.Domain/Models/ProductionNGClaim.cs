using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ProductionNGClaim
{
    public long? ClaimId { get; set; }

    public string ClaimNo { get; set; }
    public DateTime? ClaimDate { get; set; }
    public string LineCode { get; set; }
    public string Priority { get; set; }
    public string Notes { get; set; }
    public string Status { get; set; }



    [Required(ErrorMessage = "List Detail harus diisi")]
    public List<ProductionNGClaimDetail> Details { get; set; } = new List<ProductionNGClaimDetail>();
}

public class ProductionNGClaimDetail
{
   

    public string PickingNo { get; set; }
    public string LotNo { get; set; }
    public string BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string RemarksDetail { get; set; }
    public string Reason { get; set; }
    public decimal Qty { get; set; }



}
