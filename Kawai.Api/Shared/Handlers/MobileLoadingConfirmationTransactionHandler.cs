using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileLoadingConfirmationTransactionHandler : ITransactionHandler
{
    public string TransactionType => "LOADING-CONFIRMATION-MOBILE";
    private readonly IMobileLoadingConfirmationRepository _repository;
    private readonly DataLogger _logger;

    public MobileLoadingConfirmationTransactionHandler(IMobileLoadingConfirmationRepository repository, DataLogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileLoadingConfirmationSubmit>(json);

        var before = await _repository.Capture(model.InstructionNo, model.BarcodeNo);
        bool hasComplete = await _repository.Save(model, logContext.RemoteAddr ?? "", userId);
        var after = await _repository.Capture(model.InstructionNo, model.BarcodeNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Loading Confirmation",
            EntityId = model.BarcodeNo,
            ReferenceId = model.BarcodeNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Save Mobile Loading Confirmation"
        }, logContext);

        return;
    }
}

public class MobileLoadingConfirmationEvidenceBeforeTransactionHandler : ITransactionHandler
{
    public string TransactionType => "LOADING-CONFIRMATION-EVIDENCE-BEFORE";
    private readonly IMobileLoadingConfirmationRepository _repository;
    private readonly DataLogger _logger;

    public MobileLoadingConfirmationEvidenceBeforeTransactionHandler(IMobileLoadingConfirmationRepository repository, DataLogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileLoadingConfirmationEvidenceBeforeQueueSubmit>(json);
        if (model == null) throw new Exception("Invalid evidence before payload");

        var before = await _repository.CaptureEvidence(model.InstructionNo);
        await _repository.SubmitEvidenceBefore(model, userId);
        var after = await _repository.CaptureEvidence(model.InstructionNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Loading Confirmation Evidence Before",
            EntityId = model.InstructionNo,
            ReferenceId = model.InstructionNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Submit Evidence Before"
        }, logContext);

        return;
    }
}

public class MobileLoadingConfirmationEvidenceAfterTransactionHandler : ITransactionHandler
{
    public string TransactionType => "LOADING-CONFIRMATION-EVIDENCE-AFTER";
    private readonly IMobileLoadingConfirmationRepository _repository;
    private readonly DataLogger _logger;

    public MobileLoadingConfirmationEvidenceAfterTransactionHandler(IMobileLoadingConfirmationRepository repository, DataLogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileLoadingConfirmationEvidenceAfterQueueSubmit>(json);
        
        if (model == null) throw new Exception("Invalid evidence after payload");
        
        var before = await _repository.CaptureEvidence(model.InstructionNo);
        await _repository.SubmitEvidenceAfter(model, userId);
        var after = await _repository.CaptureEvidence(model.InstructionNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Loading Confirmation Evidence After",
            EntityId = model.InstructionNo,
            ReferenceId = model.InstructionNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Submit Evidence After"
        }, logContext);

        return;
    }
}  