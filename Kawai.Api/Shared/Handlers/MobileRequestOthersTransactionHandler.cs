using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileRequestOthersTransactionHandler : ITransactionHandler
{
    private readonly IMobileRequestOthersRepository _supplyRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "REQUEST-OTHERS-MOBILE";

    public MobileRequestOthersTransactionHandler(IMobileRequestOthersRepository supplyRepo, DataLogger logger)
    {
        _supplyRepo = supplyRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileRequestOthersSubmit>(json);

        var before = await _supplyRepo.Capture(model.BarcodeNo, model.RequestNo, model.LineCode);
        bool hasComplete = await _supplyRepo.Save(model, userId);
        
        var after = await _supplyRepo.Capture(model.BarcodeNo, model.RequestNo, model.LineCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Request Others",
            EntityId = model.BarcodeNo,
            ReferenceId = model.BarcodeNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Save Mobile Request Others"
        }, logContext);
    }
}
