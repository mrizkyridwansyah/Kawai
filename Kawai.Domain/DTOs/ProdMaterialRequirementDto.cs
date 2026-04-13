namespace Kawai.Domain.DTOs;

public class ProdMaterialRequirementDto
{
    public string Factory { get; set; }
    public string FactoryName { get; set; }
    public string Process { get; set; }
    public string ProcessName { get; set; }
    public string Line { get; set; }
    public string LineName { get; set; }
    public string Model { get; set; }
    public string ModelName { get; set; }
    public string LastCalculation { get; set; }
}

public class ProdMaterialRequirementDetailDto
{
    public string ParamKey { get; set; }
    public string ChildItemCode { get; set; }
    public string ChildItemName { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsName { get; set; }
    public decimal TotalReqQty { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal Shortage { get; set; }
    public string Line { get; set; }
    public string LineName { get; set; }
    public string ParentItemCode { get; set; }
    public string ParentItemName { get; set; }
    public DateTime ScheduleDate { get; set; }
    public decimal FinalReqQty { get; set; }
}
