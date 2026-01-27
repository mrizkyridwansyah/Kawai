using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/receipt-unschedule")]
[ApiController]
public class ReceiptUnscheduleController : HahaController
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IReceiptUnscheduleRepository _receiptUnscheduleRepository;
    private readonly DataLogger _logger;

    public ReceiptUnscheduleController(IReceiptRepository receiptRepository, IReceiptUnscheduleRepository receiptUnscheduleRepository, DataLogger logger)
    {
        _receiptRepository = receiptRepository;
        _receiptUnscheduleRepository = receiptUnscheduleRepository;
        _logger = logger;
    }

    [HttpPost("list-item")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _receiptUnscheduleRepository.GetListItem(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ReceiptUnschedule model)
    {
        await _receiptUnscheduleRepository.Create(model, Auth.User.UserID);

        var after = await _receiptRepository.Capture(model.Id.Value);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = model.Id.ToString(),
            ReferenceId = model.ReceiptNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });
        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] ReceiptUnschedule model)
    {
        var before = await _receiptRepository.Capture(model.Id.Value);

        await _receiptUnscheduleRepository.Update(model, Auth.User.UserID);

        var after = await _receiptRepository.Capture(model.Id.Value);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = model.Id.ToString(),
            ReferenceId = model.ReceiptNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }


}
