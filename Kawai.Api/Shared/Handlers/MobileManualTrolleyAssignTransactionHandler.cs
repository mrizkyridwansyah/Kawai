using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileManualTrolleyAssignTransactionHandler : ITransactionHandler
{
    private readonly IMobileManualTrolleyAssignRepository _manualTrolleyAssignRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "MANUAL-TROLLEY-ASSIGN-MOBILE";

    public MobileManualTrolleyAssignTransactionHandler(IMobileManualTrolleyAssignRepository manualTrolleyAssignRepo, DataLogger logger)
    {
        _manualTrolleyAssignRepo = manualTrolleyAssignRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileManualTrolleyAssign>(json);

        var before = await _manualTrolleyAssignRepo.Capture(model.RequestNo);

        await _manualTrolleyAssignRepo.Save(model, userId);

        var after = await _manualTrolleyAssignRepo.Capture(model.RequestNo);

        BackgroundJob.Enqueue<IRobotService>(service => service.CancelRequest(model.RequestNo, model.TrolleyNo));

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Manual Assign Trolley",
            EntityId = model.RequestNo,
            ReferenceId = model.RequestNo,
            Before = before,
            After = after,
            Activity = "Save Mobile Manual Assign Trolley",
            Action = DataLogAction.Update
        }, logContext);
    }
}
