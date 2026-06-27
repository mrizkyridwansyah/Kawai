namespace Kawai.Domain.Models.Mobile
{
    public class ConsumpUnscheduleSave
    {
        public string? BarcodeNo { get; set; }
        public double? QtyInput { get; set; } //qty consumpt yg diinput
        public string? Remarks { get; set; }
        public string? UserID { get; set; }
    }
}
