using Kawai.Domain.Models.Robot;

namespace Kawai.Domain.Interfaces.Robot;

public interface IRobotStockRepository
{
    Task MoveTrolley(RobotMovingTrolley payload, string robotCode);
    Task<Dictionary<string, object>> CaptureDataGrouping(string refNo);
}
