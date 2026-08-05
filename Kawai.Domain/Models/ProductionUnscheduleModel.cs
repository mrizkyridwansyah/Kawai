namespace Kawai.Domain.Models
{
    public class ProductionUnscheduleModel
    {
        public string LineCode { get; set; }
        public string ParentItemCode { get; set; }
        public string? QtyInput { get; set; }
        public string? ProductionDate { get; set; }
        public string? Remarks { get; set; }
       // public string? UserID { get; set; }
    }

    public class ProductionUnscheduleParam
    {
        public string? LineCode { get; set; }
        public string? ParentItemCode { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }

    }
}
