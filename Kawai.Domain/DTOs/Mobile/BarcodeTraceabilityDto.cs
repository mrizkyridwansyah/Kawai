namespace Kawai.Domain.DTOs.Mobile
{
    public class BarcodeTraceabilityDto
    {
        public BarcodeTraceabilityHeaderDto? Header { get; set; }

        public List<BarcodeTraceabilityListDto> Details { get; set; } = new();
    }
    public class BarcodeTraceabilityHeaderDto
    {
        public string? BarcodeNo { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? LotNo { get; set; }
        public string? Source { get; set; }
        public string? Supplier { get; set; }
        public DateTime? ReceiptDate { get; set; }
        //public string ReceiptDateStr =>
        // ReceiptDate?.ToString("dd MMM yyyy") ?? "";
        public string? LastAddress { get; set; }
        public string? StockCurrent { get; set; }
    }

    
    public class BarcodeTraceabilityListDto
    {
        public int? No { get; set; }
        public string? LineName { get; set; }
        public string? WorkstationName { get; set; }
        public DateTime? ConsumptDate { get; set; }
        public double? QtyUsed { get; set; }
        //public string ConsumptDateStr =>
        //ConsumptDate?.ToString("dd MMM yyyy HH:mm") ?? "";
    }
   

    //public class BarcodeTraceabilityDto
    //{
    //    //header
    //    public string? BarcodeNo { get; set; }
    //    public string? ItemCode {  get; set; }
    //    public string? ItemName { get; set; }
    //    public string? Source { get; set; }
    //    public string? Supplier { get; set; }
    //    public string? ReceiptDate { get; set; }
    //    public string? LotNo { get; set; }
    //    public string? LastAddress { get; set; }



    //    //detail
    //    public int? No { get; set; }
    //    public string? Line { get; set; }
    //    public string? WorkstationName { get; set; }
    //    public string? ConsumptDate { get; set; }
    //    public double? QtyUsed { get; set; }

    //}

}
