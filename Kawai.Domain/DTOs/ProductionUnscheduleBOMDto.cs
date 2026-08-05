
namespace Kawai.Domain.DTOs
{
    public class ParentItemUnscheduleDto
    {
        public string? ParentItemCode { get; set; }
        public string? ParentItemName { get; set; }
        public string? DDLDescription { get; set; }
    }
    public class ProductionUnscheduleBOMDto
    {
        public int? No { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public decimal? BomQty { get; set; }
        public decimal? QtyRequirement { get; set; }
        public decimal? CurrentStock { get; set; }
        public decimal? Remaining { get;set; }
        public string? Status { get; set; }
    }

    public class ProductionUnscheduleResultDto
    {
        public string? ProdResultID { get; set; }
        public string? LineCode { get; set; }
        public string? LineName { get; set; }
        public string? ParentItemCode { get; set; }
        public string? ParentItemName { get; set; }
        public string? ProductionDate { get; set; }
        public string? LotNo { get; set; }
        public decimal? Qty { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; }
    }

    public class ProductionUnscheduleDetailDto
    {
        public string? ProdResultID { get; set; }
        public string? ResultDetailID { get; set; }
        public string? BarcodeNo { get; set; }
        public decimal? Qty { get; set; }

    }

}
