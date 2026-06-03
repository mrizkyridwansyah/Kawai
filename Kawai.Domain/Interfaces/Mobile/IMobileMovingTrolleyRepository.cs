using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileMovingTrolleyRepository
{
    Task<List<MovingTrolleyDto>> GetDataTrolley(string trolleyNo);
    Task<StopPointDto> GetDataStopPoint(string stopPoint);
    Task Save(MobileMovingTrolley payload, string userId);
    Task<Dictionary<string, object>> Capture(string trolleyNo);
}
