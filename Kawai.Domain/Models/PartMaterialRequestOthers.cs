using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class PartMaterialRequestOthers
{
    public long? RequestId { get; set; }
    public string RequestNo { get; set; }
    public DateTime? RequestDate { get; set; }
    public string LineCode { get; set; }
    public string Status { get; set; }



    [Required(ErrorMessage = "List Detail harus diisi")]
    public List<PartMaterialRequestOthersDetail> Details { get; set; } = new List<PartMaterialRequestOthersDetail>();
}

public class PartMaterialRequestOthersDetail
{
   

    public string ItemCode { get; set; }
    public decimal RequestQty { get; set; }
   



}
