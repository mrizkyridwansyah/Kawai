using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models.Robot;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/supplyscanrequest")]
[ApiController]
public class MobileSupplyScanRequestController : HahaController
{
    private readonly IMobileSupplyScanRequestRepository _supplyscanrequestRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;
    public MobileSupplyScanRequestController(IMobileSupplyScanRequestRepository supplyscanrequestRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _supplyscanrequestRepository = supplyscanrequestRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

    [HttpGet("ddlrequestno")]
    public async Task<IActionResult> DDLSearch(string keyword, string linecode, string requestno, string warehouse)
    {
        var results = await _supplyscanrequestRepository.GetRequestNoDDL(keyword, linecode, warehouse);
        if (!string.IsNullOrEmpty(requestno))
        {
            var idList = requestno.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.RequestNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo, string requestno, string itemClass)
    {
        var result = await _supplyscanrequestRepository.GetDataBarcode(barcodeNo, requestno, itemClass);
        return Success(result);
    }

    [HttpGet("warehouseline-ddlsearch")]
    public async Task<IActionResult> WarehouseDDL(string keyword, string ids)
    {
        var results = await _supplyscanrequestRepository.GetWarehouseDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }


    [HttpGet("line-ddlsearch")]
    public async Task<IActionResult> LineDDL(string keyword, string warehousecode, string ids)
    {
        var results = await _supplyscanrequestRepository.GetLineDDL(keyword, warehousecode);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("list-material")]
    public async Task<IActionResult> GetListDetailMaterial(string requestno, string itemClass)
    {
        var result = await _supplyscanrequestRepository.GetListDetailMaterial(requestno, itemClass);
        result = result.ToList();
        return Success(result);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> GetListDetail(string warehouseCode, string requestNo, string itemCode)
    {
        var result = await _supplyscanrequestRepository.GetListDetail(warehouseCode, requestNo, itemCode);
        result = result.ToList();
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobilSupplyScanRequestSubmit model)
    {
        var message = new StockTransactionMessage<MobilSupplyScanRequestSubmit>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "SUPPLY-REQUEST-MOBILE",
            FormatMessage = "Supply Request Mobile",
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
            _transactionProducer.Publish<MobilSupplyScanRequestSubmit>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpGet("get-request-amr")]
    public async Task<IActionResult> GetRequestAMR(string requestNo)
    {
        var result = await _supplyscanrequestRepository.GetRequestAMR(requestNo);
        return Success(result);
    }

    [HttpPost("send-request-amr")]
    public async Task<IActionResult> SendRequestAMR(SendRequestAMR model)
    {
        var before = await _supplyscanrequestRepository.CaptureStatusAMR(model.RequestNoCode);

        await _supplyscanrequestRepository.SendRequestCancelAMR(model.RequestNoCode, Auth.User.UserID);

        var after = await _supplyscanrequestRepository.CaptureStatusAMR(model.RequestNoCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Send Request Complete Picking AMR",
            EntityId = model.RequestNoCode,
            ReferenceId = model.RequestNoCode,
            Before = before,
            After = after,
            Activity = "Send Request Complete Picking AMR",
            Action = DataLogAction.Update
        });

        BackgroundJob.Enqueue<IRobotService>(service => service.CompletePicking(model.RequestNoCode));

        return Success(message: "Requesting to AMR");
    }



}
