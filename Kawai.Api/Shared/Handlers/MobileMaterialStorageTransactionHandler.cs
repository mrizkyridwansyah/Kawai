using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileMaterialStorageTransactionHandler : ITransactionHandler
{
    private readonly IMobileMaterialStorageRepository _materialStorageRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "MATERIAL-STORAGE-MOBILE";

    public MobileMaterialStorageTransactionHandler(IMobileMaterialStorageRepository materialStorageRepo, DataLogger logger)
    {
        _materialStorageRepo = materialStorageRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileMaterialStorage>(json);

        var before = await _materialStorageRepo.Capture(model.RefNo);

        await _materialStorageRepo.Save(model, userId);

        var after = await _materialStorageRepo.Capture(model.RefNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Material Storage",
            EntityId = model.RefNo.ToString(),
            ReferenceId = model.RefNo.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Material Storage",
            Action = DataLogAction.Update
        }, logContext);
    }
}
