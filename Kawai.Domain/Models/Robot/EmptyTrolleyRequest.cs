using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Robot
{
    public class EmptyTrolleyRequest
    {
        [Required(ErrorMessage = "Trolley No harus diisi")]
        public string TrolleyNo { get; set; }
    }

    public class EmptyTrolley
    {
        public string TrolleyNo { get; set; }
        public string NewRefNo { get; set; }
        public string PickingNo { get; set; }
    }
}