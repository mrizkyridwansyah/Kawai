using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.DTOs
{
    public class MobileProductionResultSubmit
    {

        [Required(ErrorMessage = "Barcode tidak boleh kosong")]
        public string BarcodeNo { get; set; }
        public string ItemCode { get; set; } = "";
        public string LotNo { get; set; } = "";
        public string LineCode { get;set; } = "";
        public decimal Qty { get; set; }
    }
}