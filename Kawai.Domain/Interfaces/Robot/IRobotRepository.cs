using Kawai.Domain.Models.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Domain.Interfaces.Robot
{
    public interface IRobotRepository
    {
        //update nomer trolley dengan parameter RequestID dan TrolleyNo
        Task SetTrolleyAsync(SetTrolleyRequest payload);

        //complete status
        Task CompleteStatusAsync(CompleteStatusRequest payload);

        //update empty trolley dengan parameter TrolleyNo
        Task EmptyTrolleyAsync(EmptyTrolleyRequest payload);

        Task<Dictionary<string, object>> CaptureSetTrolley(SetTrolleyRequest payload);
        Task<Dictionary<string, object>> CaptureCompleteStatus(CompleteStatusRequest payload);

        Task<Dictionary<string, object>> CaptureEmptyTrolley(EmptyTrolleyRequest payload);
        Task<string> GetListData();


    }
}
