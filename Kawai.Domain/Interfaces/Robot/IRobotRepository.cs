using Kawai.Domain.DTOs.Robot;
using Kawai.Domain.Models.Robot;

namespace Kawai.Domain.Interfaces.Robot
{
    public interface IRobotRepository
    {
        //update nomer trolley dengan parameter RequestID dan TrolleyNo
        Task SetTrolleyAsync(SetTrolleyRequest payload);

        //update nomer trolley dengan parameter RequestID dan TrolleyNo
        Task MoveTrolley(MovingTrolleyRequest payload);

        //update empty trolley dengan parameter TrolleyNo
        Task EmptyTrolleyAsync(EmptyTrolley payload);

        Task<Dictionary<string, object>> CaptureSetTrolley(SetTrolleyRequest payload);
        Task<Dictionary<string, object>> CaptureStockTrolley(string trolleyNo);


        /* kaya nya ga kepake */
        Task CompleteStatusAsync(CompleteStatusRequest payload);
        Task<List<SupplyRequestDto>> GetListData();
        Task<Dictionary<string, object>> CaptureCompleteStatus(CompleteStatusRequest payload);

    }
}
