using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.DTOs
{
    public class MobileProductionResultScanDto
    {
        public string BarcodeNo { get; set; } = "";
        public string ItemCode { get; set; } = "";
        public string ItemName { get; set; } = "";
        public string LotNo { get; set; } = "";
        public string LineCode { get; set; } = "";
        public string LineName { get; set; } = "";
        public decimal Qty { get; set; } 
    }
   
}
