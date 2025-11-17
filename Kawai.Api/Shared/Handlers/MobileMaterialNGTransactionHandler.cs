using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileMaterialNGTransactionHandler : ITransactionHandler
{
    private readonly IMobileMaterialNGRepository _materialNGRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "MATERIAL-NG-MOBILE";

    public MobileMaterialNGTransactionHandler(IMobileMaterialNGRepository materialNGRepo, DataLogger logger)
    {
        _materialNGRepo = materialNGRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileMaterialNG>(json);

        var before = model.InspectionId.HasValue ? await _materialNGRepo.Capture(model.InspectionId.Value) : null;

        await _materialNGRepo.Save(model, userId);

        var after = await _materialNGRepo.Capture(model.InspectionId.Value);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Material NG",
            EntityId = model.InspectionId.ToString(),
            ReferenceId = model.InspectionId.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Material NG",
            Action = DataLogAction.Create
        });
    }
}
