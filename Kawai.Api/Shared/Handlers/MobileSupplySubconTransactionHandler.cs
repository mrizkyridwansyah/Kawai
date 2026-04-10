using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileSupplySubconTransactionHandler : ITransactionHandler
{
    private readonly IMobileSupplySubconRepository _supplyRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "SUPPLY-SUBCON-MOBILE";

    public MobileSupplySubconTransactionHandler(IMobileSupplySubconRepository supplyRepo, DataLogger logger)
    {
        _supplyRepo = supplyRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileSupplySubcon>(json);

        var before = await _supplyRepo.Capture(model.RequestNoCode, model.ItemCode);
        await _supplyRepo.Save(model, userId);
        var after = await _supplyRepo.Capture(model.RequestNoCode, model.ItemCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Supply Subcon Scan",
            EntityId = model.RequestNoCode,
            ReferenceId = model.RequestNoCode,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Save Mobile Supply Subcon Scan"
        }, logContext);
    }
}
