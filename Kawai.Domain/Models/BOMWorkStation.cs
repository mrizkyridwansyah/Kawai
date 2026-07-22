using Kawai.Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class BOMWorkStation
{


    public List<BOMWorkStationListHeader> Header { get; set; }
    public List<BOMWorkStationList> Details { get; set; }


}

public class CopyBomWorkstation
{
    public string FromLine { get; set; }
    public string ToLine { get; set; }
    public string ItemCode { get; set; }
     
}


public class BOMWorkStationList
{
    public string ChildItem_Code { get; set; }
    public decimal? Qty { get; set; }
    public bool? AllowSetting { get; set; } = false;
    
}

public class BOMWorkStationListHeader
{
    public string FactoryCode { get; set; }
    public string LineCode { get; set; }
    public string ModelCls { get; set; }
    public string ParentItem_Code { get; set; }
    public string ProcessCode { get; set; }

     public decimal? QtySet { get; set; }

    public string Trolley_Cls { get; set; }

    public string WorkStationCode { get; set; }
    

}

public class BOMWSImport
{
    public BOMWSHeaderImport Header { get; set; }
    public List<BOMWSDetailImport> Details { get; set; } = new();
}

public class BOMWSHeaderImport
{
    public string LineCode { get; set; }
    public string ParentItemCode { get; set; }
    public string WorkStationCode { get; set; }
    public string TrolleyCls { get; set; }
    public string Errors { get; set; } = "";
}

public class BOMWSDetailImport : ImportBase
{

 
    public string ChildItemCode { get; set; }
    public decimal Qty { get; set; }


}

