using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileAssignStorageTransactionHandler : ITransactionHandler
{
    private readonly IMobileAssignStorageRepository _assignStorageRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "ASSIGN-STORAGE-MOBILE";

    public MobileAssignStorageTransactionHandler(IMobileAssignStorageRepository assignStorageRepo, DataLogger logger)
    {
        _assignStorageRepo = assignStorageRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileAssignStorage>(json);

        var before = await _assignStorageRepo.Capture(model.RefNo);

        await _assignStorageRepo.Save(model, userId);

        var after = await _assignStorageRepo.Capture(model.RefNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Assign Storage",
            EntityId = model.RefNo.ToString(),
            ReferenceId = model.RefNo.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Assign Storage",
            Action = DataLogAction.Create
        }, logContext);
    }
}
