using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileAssignToTrolleyRepository
{
    Task<AssignToTrolleyDto> GetDataTrolley(string trolleyNo);
    Task Save(MobileAssignToTrolley payload, string userId);
}
