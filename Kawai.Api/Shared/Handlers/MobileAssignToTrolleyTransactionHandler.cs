using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileAssignToTrolleyTransactionHandler : ITransactionHandler
{
    private readonly IMobileAssignToTrolleyRepository _assignToTrolleyByBarcodeRepo;
    private readonly IMobileLoadingTrolleyRepository _loadingTrolleyByBarcodeRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "ASSIGN-TO-TROLLEY-BY-BARCODE-MOBILE";

    public MobileAssignToTrolleyTransactionHandler(IMobileAssignToTrolleyRepository assignToTrolleyByBarcodeRepo, IMobileLoadingTrolleyRepository loadingTrolleyByBarcodeRepo, DataLogger logger)
    {
        _assignToTrolleyByBarcodeRepo = assignToTrolleyByBarcodeRepo;
        _loadingTrolleyByBarcodeRepo = loadingTrolleyByBarcodeRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileAssignToTrolley>(json);

        var before = await _loadingTrolleyByBarcodeRepo.Capture(model.TrolleyNo);

        await _assignToTrolleyByBarcodeRepo.Save(model, userId);

        var after = await _loadingTrolleyByBarcodeRepo.Capture(model.TrolleyNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Assign To Trolley By Barcode",
            EntityId = model.TrolleyNo,
            ReferenceId = model.TrolleyNo,
            Before = before,
            After = after,
            Activity = "Save Mobile Assign To Trolley By Barcode",
            Action = DataLogAction.Update
        }, logContext);
    }
}
