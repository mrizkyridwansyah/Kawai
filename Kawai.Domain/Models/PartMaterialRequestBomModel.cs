using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class PartMaterialRequestBomModel
{
    public string WarehouseCode { get; set; }
    public long? RequestId { get; set; }
    public string PONumber { get; set; }
    public DateTime PODate { get; set; }
    public string ItemCode { get; set; }
    public decimal RequestSetQty { get; set; }
}


public class PartMaterialRequestBomHeaderModel
{
    public long? RequestId { get; set; }
    public string RequestNo { get; set; }

    [Required(ErrorMessage = "DN Number tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "DN Number tidak boleh lebih dari 50 karakter")]
    public string DNNumber { get; set; }

    [Required(ErrorMessage = "DN Date tidak boleh kosong")]
    public DateTime? DNDate { get; set; }

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

    public List<PartMaterialRequestBomModel> Details { get; set; }
}

public class PartMaterialRequestBomDetailModel
{
    public long IDSeq { get; set; }
    public string ChilItemCode { get; set; }
    public decimal? ReqQty { get; set; }


}
