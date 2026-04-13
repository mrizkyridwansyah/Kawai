using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileLoadingTrolleyTransactionHandler : ITransactionHandler
{
    private readonly IMobileLoadingTrolleyRepository _loadingTrolleyRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "LOADING-TROLLEY-MOBILE";

    public MobileLoadingTrolleyTransactionHandler(IMobileLoadingTrolleyRepository LoadingTrolleyRepo, DataLogger logger)
    {
        _loadingTrolleyRepo = LoadingTrolleyRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileLoadingTrolley>(json);

        await _loadingTrolleyRepo.ScanBarcode(model, userId);

        var after = await _loadingTrolleyRepo.Capture(model.TrolleyNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Loading Trolley",
            EntityId = model.TrolleyNo.ToString(),
            ReferenceId = model.TrolleyNo.ToString(),
            After = after,
            Activity = "Save Mobile Loading Trolley",
            Action = DataLogAction.Create
        }, logContext);
    }
}
