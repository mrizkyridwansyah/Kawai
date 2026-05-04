
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Robot
{
    public class SetTrolleyRequest
    {
        [Required(ErrorMessage = "RequestID harus diisi")]
        public string RequestID { get; set; }

        [Required(ErrorMessage = "Trolley No harus diisi")]
        public string TrolleyNo { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
