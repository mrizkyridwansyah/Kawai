using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using System.Text.Json;

namespace Kawai.Api.Shared.Handlers;

public class ReceiptTransactionHandler: ITransactionHandler
{
    private readonly IReceiptRepository _receiptRepo;
    private readonly DataLogger _logger;

    public string TransactionType => "RECEIPT";

    public ReceiptTransactionHandler(IReceiptRepository receiptRepo, DataLogger logger)
    {
        _receiptRepo = receiptRepo;
        _logger = logger;
    }

    public async Task HandleAsync(object payload, string userId)
    {
        var json = JsonSerializer.Serialize(payload);
        var model = JsonSerializer.Deserialize<Receipt>(json);

        await _receiptRepo.Create(model, userId);

        if (model.Id.HasValue)
        {
            var after = await _receiptRepo.Capture(model.Id.Value);
            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Part Receipt Material",
                EntityId = model.Id.ToString(),
                ReferenceId = model.ReceiptNo,
                After = after,
                Action = DataLogAction.Create
            });
        }
    }
}
