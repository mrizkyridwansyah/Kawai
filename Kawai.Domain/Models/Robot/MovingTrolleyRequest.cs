using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Domain.Models.Robot
{
    public class MovingTrolleyRequest
    {
        [Required(ErrorMessage = "Trolley No harus diisi")]
        public string TrolleyNo { get; set; }

        [Required(ErrorMessage = "Address Code harus diisi")]
        public string AddressCode { get; set; }

        [Required(ErrorMessage = "Stop Point Code harus diisi")]
        public string StopPointCode { get; set; }

    }
}
