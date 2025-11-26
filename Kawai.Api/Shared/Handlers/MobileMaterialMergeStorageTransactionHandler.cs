using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileMaterialMergeStorageTransactionHandler : ITransactionHandler
{
    private readonly IMobileMaterialStorageRepository _materialStorageRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "MATERIAL-MERGE-STORAGE-MOBILE";

    public MobileMaterialMergeStorageTransactionHandler(IMobileMaterialStorageRepository materialStorageRepo, DataLogger logger)
    {
        _materialStorageRepo = materialStorageRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileMaterialMergeStorage>(json);

        await _materialStorageRepo.SaveMerge(model, userId);

        var after = await _materialStorageRepo.Capture(model.RefNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Material Merge Storage",
            EntityId = model.RefNo.ToString(),
            ReferenceId = model.RefNo.ToString(),
            Before = null,
            After = after,
            Activity = "Save Mobile Material Merge Storage",
            Action = DataLogAction.Create
        }, logContext);
    }
}
