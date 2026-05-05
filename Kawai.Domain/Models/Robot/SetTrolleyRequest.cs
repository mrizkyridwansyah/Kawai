
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Robot
{
    public class SetTrolleyRequest
    {
        [Required(ErrorMessage = "RequestID harus diisi")]
        public string RequestID { get; set; }

        public string TrolleyNo { get; set; }

        public string Status { get; set; }
    }
}
