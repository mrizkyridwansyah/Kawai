using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class QualityCheck
{

    public string Id { get; set; }
    public string Supplier_Code { get; set; }
    public string DN_No { get; set; }
    public string Item_Code { get; set; }
    public string Item_Name { get; set; }
    public string Qty { get; set; }
    public string Inspection_Date { get; set; }
    public string Inspection_User { get; set; }
    public string QC_Status { get; set; }
    public string Remarks { get; set; }
    public string QC_Photo { get; set; }
    public string Status { get; set; }
    public DateTime? Receipt_Date { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }
    public string QC_Status_Descs { get; set; }


}
