
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Robot
{
    public class CompleteStatusRequest
    {
        
        public string RequestID { get; set; }
        public string TrolleyNo { get; set; }

        public string StopPoint { get; set; }

        public int Status { get; set; } //0/1

    }
}
