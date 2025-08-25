using DocumentFormat.OpenXml.InkML;
using Kawai.Api.CronJobs;
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
    private readonly ITransactionProducer _transactionProducer;
    //private readonly StockCalculation _stockCalculation;
    private readonly DataLogger _logger;

    //public ReceiptController(IReceiptRepository receiptRepository, DataLogger logger, StockCalculation stockCalculation, ITransactionProducer transactionProducer)
    public ReceiptController(IReceiptRepository receiptRepository, DataLogger logger, ITransactionProducer transactionProducer)
    {
        _receiptRepository = receiptRepository;
        //_stockCalculation = stockCalculation;
        _logger = logger;
        _transactionProducer = transactionProducer;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var result = await _receiptRepository.GetDetail(id);
        return Success(result);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> GetListDetail(long id)
    {
        var results = await _receiptRepository.GetListDetail(id);
        return Success(results);
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

    /*
    [HttpPost("create-using-mutation")]
    public async Task<IActionResult> CreateUsingMutation([FromBody] Receipt model)
    {
        await _receiptRepository.CreateUsingMutation(model, Auth.User.UserID);

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

        _stockCalculation.AddTransaction();
        return Pending(after);
    }
    */

    [HttpPost("create-using-rabbitmq")]
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
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(long id, string receiptNo)
    {
        var before = await _receiptRepository.Capture(id);
        await _receiptRepository.Remove(id);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = id.ToString(),
            ReferenceId = receiptNo,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }
}
