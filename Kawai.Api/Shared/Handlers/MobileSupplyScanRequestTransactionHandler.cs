using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileSupplyScanRequestTransactionHandler : ITransactionHandler
{
    private readonly IMobileSupplyScanRequestRepository _supplyRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "SUPPLY-REQUEST-MOBILE";

    public MobileSupplyScanRequestTransactionHandler(IMobileSupplyScanRequestRepository supplyRepo, DataLogger logger)
    {
        _supplyRepo = supplyRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobilSupplyScanRequestSubmit>(json);

        var before = await _supplyRepo.Capture(model.BarcodeNo, model.RequestNoCode, model.ItemClass);
        await _supplyRepo.Save(model, userId);
        var after = await _supplyRepo.Capture(model.BarcodeNo, model.RequestNoCode, model.ItemClass);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Supply Request Scan",
            EntityId = model.BarcodeNo,
            ReferenceId = model.BarcodeNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Save Mobile Supply Request Scan"
        }, logContext);
    }
}
