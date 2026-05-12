using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Robot
{
    public class MovingTrolleyRequest
    {
        [Required(ErrorMessage = "Trolley No harus diisi")]
        public string TrolleyNo { get; set; }

        [Required(ErrorMessage = "From Address Code harus diisi")]
        public string FromAddressCode { get; set; }

        [Required(ErrorMessage = "To Address Code harus diisi")]
        public string ToAddressCode { get; set; }

        //public string RobotCode { get; set; }   
    }
}
