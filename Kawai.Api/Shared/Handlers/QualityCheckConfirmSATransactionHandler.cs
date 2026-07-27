using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class QualityCheckConfirmSATransactionHandler : ITransactionHandler
{
    private readonly IQualityCheckRepository _qualityCheckRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "IQC-RESULT-APPROVAL-SA";

    public QualityCheckConfirmSATransactionHandler(IQualityCheckRepository qualityCheckRepo, DataLogger logger)
    {
        _qualityCheckRepo = qualityCheckRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<QualityCheckConfirmSA>(json);

        var before = await _qualityCheckRepo.Capture(model.InspectionId);

        if (model.ProcessUnapprove.HasValue && model.ProcessUnapprove.Value)
            await _qualityCheckRepo.ApprovalSAUnapprove(model, userId);
        else
            await _qualityCheckRepo.ApprovalSA(model, userId);

        var after = await _qualityCheckRepo.Capture(model.InspectionId);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Quality Check IQC",
            EntityId = model.InspectionId.ToString(),
            ReferenceId = model.InspectionId.ToString(),
            Action = DataLogAction.Update,
            Activity = "Approval Quality Check SA",
            Before = before,
            After = after
        }, logContext);
    }
}
