using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileBarcodeSplitTransactionHandler : ITransactionHandler
{
    private readonly IMobileBarcodeSplitRepository _barcodeSplitRepository;
    private readonly DataLogger _logger;

    public string TransactionType => "BARCODE-SPLIT-MOBILE";

    public MobileBarcodeSplitTransactionHandler(IMobileBarcodeSplitRepository barcodeSplitRepository, DataLogger logger)
    {
        _barcodeSplitRepository = barcodeSplitRepository;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileBarcodeSplit>(json);

        var before = await _barcodeSplitRepository.Capture(model.BarcodeNo);

        await _barcodeSplitRepository.Save(model, userId);

        var after = await _barcodeSplitRepository.Capture(model.BarcodeNo);

        if(!String.IsNullOrEmpty(model.BarcodeNoNew))
        {
            var after2 = await _barcodeSplitRepository.Capture(model.BarcodeNoNew);
            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Mobile Barcode Split",
                EntityId = model.BarcodeNo,
                ReferenceId = model.BarcodeNo,
                After = after,
                Activity = "Save Mobile Barcode Split",
                Action = DataLogAction.Create
            }, logContext);
        }

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Barcode Split",
            EntityId = model.BarcodeNo,
            ReferenceId = model.BarcodeNo,
            Before = before,
            After = after,
            Activity = "Save Mobile Barcode Split",
            Action = DataLogAction.Update
        }, logContext);
    }
}
