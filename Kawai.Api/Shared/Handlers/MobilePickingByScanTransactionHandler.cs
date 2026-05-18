using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobilePickingByScanTransactionHandler : ITransactionHandler
{
    private readonly IMobilePickingByScanRepository _pickingRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "PICKING-MOBILE";

    public MobilePickingByScanTransactionHandler(
        IMobilePickingByScanRepository pickingRepo,
        DataLogger logger)
    {
        _pickingRepo = pickingRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobilePickingByScanSubmit>(json);

        var before = await _pickingRepo.Capture(model.InstructionNo, model.BarcodeNo);
        bool hasComplete = await _pickingRepo.Save(model, logContext.RemoteAddr ?? "", userId);
        var after = await _pickingRepo.Capture(model.InstructionNo, model.BarcodeNo);

        /// LOG
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Picking By Scan",
            EntityId = model.BarcodeNo,
            ReferenceId = model.BarcodeNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Save Mobile Picking By Scan"
        }, logContext);
    }
}