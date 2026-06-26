using Kawai.Domain.DTOs.Mobile;
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


        Task<List<SupplyRequestDto>> GetListData(string reqId);
        Task<SupplyRequestCompleteDto> GetRequestData(string reqId, string stopPoint);

        Task<SupplyScanRequestInfoAMRCls> GetSupplyScanRequestInfo(string requestNo);
        Task<SupplyScanRequestSubLineDto> GetDataToSendRequestSubLine(string requestNo, string trolleyNo, string stopPoint);
    }
}
