using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ShippingInstructionMaster
{
    [Required(ErrorMessage = "Cust Code tidak boleh kosong")]
    public string Cust_Code { get; set; }

    [Required(ErrorMessage = "PO No tidak boleh kosong")]
    public string PO_No { get; set; }

    [Required(ErrorMessage = "SI No tidak boleh kosong")]
    public string SI_No { get; set; }

    [Required(ErrorMessage = "PO Delivery Date tidak boleh kosong")]
    public DateTime PO_DelivDate { get; set; }

    [Required(ErrorMessage = "SI Date tidak boleh kosong")]
    public DateTime SI_Date { get; set; }

    public DateTime Register_Date { get; set; }
    public string Register_By { get; set; }
    public DateTime Update_Date { get; set; }
    public string Update_By { get; set; }
}

public class ShippingInstructionDetail
{
    [Required(ErrorMessage = "SI No tidak boleh kosong")]
    public string SI_No { get; set; }

    [Required(ErrorMessage = "Item Code tidak boleh kosong")]
    public string Item_Code { get; set; }
    public string Item_Name { get; set; }

    public string Unit_Cls { get; set; }
    public string Unit_Desc { get; set; }

    public DateTime DelivDate { get; set; }

    [Required(ErrorMessage = "Serial No tidak boleh kosong")]
    public string Serial_No { get; set; }
    public string Address { get; set; }
    public string IsPicking { get; set; } = "0";
    public DateTime Picking_Date { get; set; }
    public string Picking_By { get; set; }
    public DateTime Register_Date { get; set; }
    public string Register_By { get; set; }
    public DateTime Update_Date { get; set; }
    public string Update_By { get; set; }
}

public class ShippingInstructionRequest
{
    public string PONo { get; set; }
    public int POSeqNo { get; set; }
    public string ItemCode { get; set; }
    public string SerialNoFrom { get; set; }
    public string SerialNoTo { get; set; }
    public DateTime SIDate { get; set; }
}

public class ShippingPickingRequest
{
    public string PONo { get; set; }
    public int POSeqNo { get; set; }
    public string ItemCode { get; set; }
    public string SerialNo { get; set; }
}
