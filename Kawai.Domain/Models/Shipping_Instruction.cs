using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;


public class ShippingInstructionPicking
{


    public List<ShippingInstructionPickingHeader> Header { get; set; }
    public List<ShippingInstructionPickingDetail> Details { get; set; }


}

 

public class ShippingInstructionPickingDetail
{
    public string SerialNo { get; set; }
    public bool? AlreadyPicking { get; set; } = false;
   
    

}

public class ShippingInstructionPickingHeader
{
 

    public string ShippingInstructionNo { get; set; }
    public string ItemCode { get; set; }
    public string PONumber { get; set; }
    public int PO_SeqNo { get; set; }
 


}


public class ShippingInstruction
{
 
    public string ShippingInstructionNo { get; set; }

    [Required(ErrorMessage = "SI Date harus diisi")]
    public DateTime? ShippingInstructionDate { get; set; }

    [Required(ErrorMessage = "Order Number harus diisi")]
    public string PONumber { get; set; }

    public string Supplier { get; set; }

    [Required(ErrorMessage = "List Detail harus diisi")]
    public List<ShippingInstruction_Detail> Details { get; set; } = new List<ShippingInstruction_Detail>();
}

public class ShippingInstruction_Detail
{
 
    public string Item_Code { get; set; }
    public int PO_SeqNo { get; set; }
    public decimal Qty { get; set; }
    public string SerialNo_From { get; set; }
    public string SerialNo_To { get; set; }



}
