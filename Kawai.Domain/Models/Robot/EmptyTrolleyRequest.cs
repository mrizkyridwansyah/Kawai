using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Domain.Models.Robot
{
    public class EmptyTrolleyRequest
    {
        [Required(ErrorMessage = "Trolley No harus diisi")]
        public string TrolleyNo { get; set; }
    }
}
