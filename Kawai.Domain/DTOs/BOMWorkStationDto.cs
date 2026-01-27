namespace Kawai.Domain.DTOs;

public class BOMWorkStationDto: DataTableDto
{
    public string Factory { get; set; }
    public string Process { get; set; }
    public string Line { get; set; }
    public string WorkStationCode { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string ModelCls { get; set; }
    public string Description { get; set; }
    public string DDLDescription { get; set; }
    public string WorkStationName { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
   
}

public class ParentBOMWorkStationDto
{
    public bool AllowSetting { get; set; }
    public string ChildItem_Code { get; set; }
    public string ChildItem_Name { get; set; }
    public string Qty { get; set; }
    public string Unit_Cls { get; set; }
    public string Unit_Descs { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string LastUser { get; set; }
    public DateTime? LastUpdate { get; set; }

    public string ParentItem_Code { get; set; }
    public string WorkStationCode { get; set; }
    public string WorkStationName { get; set; }
     

}

public class BOMWorkStationHeaderDto
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

public class BOMWorkStationDetailDto
{
    public bool AllowSetting { get; set; }
    public string ChildItem_Code { get; set; }
    public string ChildItem_Name { get; set; }
    public string Qty { get; set; }
    public string Unit_Cls { get; set; }
    public string Unit_Descs { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string LastUser { get; set; }
    public DateTime? LastUpdate { get; set; }
 

}

