using Kawai.Domain.Models;
using System.Reflection;
using System;

namespace Kawai.Domain.DTOs;

// INI DIPAKE DI WEB & MOBILE
public class PartMaterialRequestOthersDto: DataTableDto
{
    
    public long? RequestID { get; set; }
    public string RequestNo { get; set; }
    public DateTime RequestDate { get; set; }
    public string FactoryCode { get; set; }
    public string FactoryName { get; set; }
    public string ManufactureCode { get; set; }
    public string ManufactureName { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public decimal TotalQty { get; set; }
    public decimal TotalItem { get; set; }
    public string Status { get; set; }
    public string Request_User { get; set; }
    public DateTime Request_Date { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string RegisterUser { get; set; }
}

 
// INI DIPAKE DI WEB & MOBILE
public class PartMaterialRequestOthersDetailDto : DataTableDto
{
   
    public long DetailID { get; set; }
    public long RequestID { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string Storage { get; set; }
    public string StorageDescs { get; set; }
    public decimal QtyPacking { get; set; }
    public decimal AvailableStock { get; set; }
    public decimal AvgNeedPlan { get; set; }
    public decimal RequestQty { get; set; }
    public string Status { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}

public class PartMaterialRequestOthersHistoryDto : DataTableDto
{
    public long? RequestID { get; set; }
    public string RequestNo { get; set; }
    public DateTime RequestDate { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public decimal TotalMaterial { get; set; }
    public decimal TotalQty { get; set; }
    public decimal FulfilledMaterial { get; set; }
    public string Status { get; set; }
 
}

public class PartMaterialRequestOthersListScanDto : DataTableDto
{
    public long? RequestID { get; set; }
    public string RequestNo { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public decimal RequestQty { get; set; }
    public decimal SendQty { get; set; }
    public string Status { get; set; }

}

