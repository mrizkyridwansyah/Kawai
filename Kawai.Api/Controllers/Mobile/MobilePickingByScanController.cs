using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models.Robot;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/pickingbyscan")]
[ApiController]
public class MobilePickingByScanController : HahaController
{
    private readonly IMobilePickingByScanRepository _repository;
    private readonly ITransactionProducer _transaction;
    private readonly DataLogger _logger;

    public MobilePickingByScanController(IMobilePickingByScanRepository repository, ITransactionProducer transaction, DataLogger logger)
    {
        _repository = repository;
        _transaction = transaction;
        _logger = logger;
    }

    [HttpGet("ddl-instruction")]
    public async Task<IActionResult> GetInstructionDDL(string keyword, string ids)
    {
        var results = await _repository.GetInstructionDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(x => x.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.InstructionNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("list-shipping")]
    public async Task<IActionResult> GetListDetailShipping(string instructionNo, string keyword)
    {
        var result = await _repository.GetListDetailShipping(instructionNo, keyword);
        result = result.ToList();
        return Success(result);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> GetListDetail(string instructionNo, string barcodeNo, string partNo, string serialNo)
    {
        var result = await _repository.GetListDetail(instructionNo, barcodeNo, partNo, serialNo);
        result = result.ToList();
        return Success(result);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo, string instructionNo)
    {
        var result = await _repository.GetDataBarcode(barcodeNo, instructionNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobilePickingByScanSubmit model)
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader))
        {
            throw new Exception("Authorization header missing");
        }

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "";
        if (Auth.User == null)
        {
            throw new Exception("Auth.User is NULL");
        }
        var payload = new
        {
            model.InstructionNo,
            model.BarcodeNo,
            DeviceID = remoteIp,
            Auth.User.UserID
        };

        var message = new StockTransactionMessage<MobilePickingByScanSubmit>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "PICKING-MOBILE",
            FormatMessage = "Picking Mobile",
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

        try
        {
            _transaction.Publish<MobilePickingByScanSubmit>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

}