namespace Kawai.Domain.DTOs
{
    public class ExpiredListRequestDto : DataTableDto
    {
        //public string ItemCode { get; set; }
        //public string LineCode { get; set; }
        public DateTime? ExpiredUntil { get; set; }

        public Dictionary<string, string> Sorts { get; set; }
        public int Page { get; set; }
        public int Length { get; set; }
    }

    public class ExpiredListDto : DataTableDto
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartNo { get; set; }
        public string WHCode { get; set; }
        public string WHName { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal ExpireDay { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public string LotNo { get; set; }
    }

}
