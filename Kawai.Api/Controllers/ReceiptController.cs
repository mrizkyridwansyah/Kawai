using Kawai.Api.Services;
using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/receipt")]
[ApiController]
public class ReceiptController : HahaController
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly DataLogger _logger;

    public ReceiptController(IReceiptRepository receiptRepository, DataLogger logger)
    {
        _receiptRepository = receiptRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(long id)
    {
        var result = await _receiptRepository.GetDataHeader(id);
        return Success(result);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> GetListDetail(long id)
    {
        var results = await _receiptRepository.GetListDetail(id);
        return Success(results);
    }

    [HttpPost("list-po-detail")]
    public async Task<IActionResult> GetListPODetail([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.GetListPODetail(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Receipt model)
    {
        await _receiptRepository.Create(model, Auth.User.UserID);

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
    public async Task<IActionResult> Update([FromBody] Receipt model)
    {
        var before = await _receiptRepository.Capture(model.Id.Value);

        await _receiptRepository.Update(model, Auth.User.UserID);

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

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string sourceMenu, string ids)
    {
        var results = await _receiptRepository.DDLSearch(keyword, supplier, periodFrom, periodUntil, status, sourceMenu);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.Id.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    /*
    [HttpPost("create-using-rabbitmq")]
    //[Idempotent]
    public async Task<IActionResult> CreateUsingRabbitMQ([FromBody] Receipt model)
    {
        var message = new StockTransactionMessage<Receipt>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "RECEIPT",
            FormatMessage = "Receipt DN No. : " + model.DNNumber,
            Payload = model,
            LogContext = new LogContext
            {
                Method = HttpContext.Request.Method,
                RequestPath = HttpContext.Request.Path,
                RemoteAddr = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                UserID = Auth.User.UserID,
                FullName = Auth.User.FullName
            }
        };

        _transactionProducer.Publish<Receipt>(message);
        return Pending(message);
    }
    */
}
