using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Robot;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class RobotMovingTrolleyTransactionHandler : ITransactionHandler
{
    private readonly IRobotStockRepository _repo;
    private readonly DataLogger _logger;

    public string TransactionType => "ROBOT-MOVING-TROLLEY";

    public RobotMovingTrolleyTransactionHandler(IRobotStockRepository repo, DataLogger logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<RobotMovingTrolley>(json);

        var before = await _repo.CaptureDataGrouping(model.TrolleyCode);
        await _repo.MoveTrolley(model, logContext.UserID);
        var after = await _repo.CaptureDataGrouping(model.TrolleyCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Robot - Moving Trolley",
            EntityId = model.TrolleyCode,
            ReferenceId = model.TrolleyCode,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Robot Moving Trolley"
        }, logContext);
    }
}
