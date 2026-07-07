using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileMovingTrolleyRepository
{
    Task<List<MovingTrolleyDto>> GetDataTrolley(string trolleyNo);
    Task<StopPointDto> GetDataStopPoint(string stopPoint);
    Task CheckValidation(MobileMovingTrolley payload);
    Task Save(MobileMovingTrolley payload, string userId);
    Task<Dictionary<string, object>> Capture(string trolleyNo);
    Task SendRequestUnbindRackAMR(string requestNo, string trolleyNo, string userId);
    Task UpdateStatusUnbindRackAMR(string requestNo, string trolleyNo, string lastStatus);
    Task SendRequestSubLineAMR(string requestNo, string trolleyNo, string stopPoint, string userId);
    Task UpdateStatusAMRSendRequestSubLine(string requestNo, string trolleyNo, string stopPoint, string lastStatus);
}
