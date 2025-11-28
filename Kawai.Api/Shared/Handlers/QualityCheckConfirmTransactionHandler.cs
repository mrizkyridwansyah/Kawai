using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class QualityCheckConfirmTransactionHandler : ITransactionHandler
{
    private readonly IQualityCheckRepository _qualityCheckRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "IQC-RESULT-CONFIRM";

    public QualityCheckConfirmTransactionHandler(IQualityCheckRepository qualityCheckRepo, DataLogger logger)
    {
        _qualityCheckRepo = qualityCheckRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<QualityCheckConfirm>(json);

        var before = await _qualityCheckRepo.Capture(model.InspectionId);

        await _qualityCheckRepo.Confirm(model, userId);

        var after = await _qualityCheckRepo.Capture(model.InspectionId);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Quality Check IQC",
            EntityId = model.InspectionId.ToString(),
            ReferenceId = model.InspectionId.ToString(),
            Action = DataLogAction.Update,
            Activity = "Confirm Quality Check",
            Before = before,
            After = after
        }, logContext);
    }
}
