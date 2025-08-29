using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class MobileReceiptVerifyTransactionHandler : ITransactionHandler
{
    private readonly IReceiptRepository _receiptRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "RECEIPT-VERIFY-MOBILE";

    public MobileReceiptVerifyTransactionHandler(IReceiptRepository receiptRepo, DataLogger logger)
    {
        _receiptRepo = receiptRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, LogContext logContext, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<MobileReceipt>(json);

        var before = await _receiptRepo.CaptureDataBarcode(model.Id);
        await _receiptRepo.Verify(model, true, userId);
        var after = await _receiptRepo.CaptureDataBarcode(model.Id);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile - Receipt",
            EntityId = model.Id.ToString(),
            ReferenceId = model.BarcodeNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Verify Receiving Barcode"
        }, logContext);
    }
}
