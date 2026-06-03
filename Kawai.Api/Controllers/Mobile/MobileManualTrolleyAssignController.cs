using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models.Robot;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/manual-trolley-assign")]
[ApiController]
public class MobileManualTrolleyAssignController : HahaController
{
    private readonly IMobileManualTrolleyAssignRepository _manualTrolleyAssignRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;

    public MobileManualTrolleyAssignController(IMobileManualTrolleyAssignRepository manualTrolleyAssignRepository, DataLogger logger, ITransactionProducer transactionProducer)
    {
        _manualTrolleyAssignRepository = manualTrolleyAssignRepository;
        _logger = logger;
        _transactionProducer = transactionProducer;
    }

    [HttpGet("ddl-line")]
    public async Task<IActionResult> LineDDL(string keyword, string itemClass, string ids)
    {
        var results = await _manualTrolleyAssignRepository.GetLineDDL(keyword, itemClass);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-requestno")]
    public async Task<IActionResult> DDLSearch(string keyword, string itemClass, string lineCode, string requestno)
    {
        var results = await _manualTrolleyAssignRepository.GetRequestNoDDL(keyword, itemClass, lineCode);
        if (!string.IsNullOrEmpty(requestno))
        {
            var idList = requestno.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.RequestNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("data-request")]
    public async Task<IActionResult> GetDataRequest(string requestNo)
    {
        var result = await _manualTrolleyAssignRepository.GetDataRequest(requestNo);
        return Success(result);
    }

    [HttpGet("data-trolley")]
    public async Task<IActionResult> GetDataTrolley(string requestNo, string trolleyNo)
    {
        var result = await _manualTrolleyAssignRepository.GetDataTrolley(requestNo, trolleyNo);
        return Success(result);
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckValidation(MobileManualTrolleyAssign model)
    {
        var result = await _manualTrolleyAssignRepository.CheckValidation(model);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileManualTrolleyAssign model)
    {
        var message = new StockTransactionMessage<MobileManualTrolleyAssign>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "MANUAL-TROLLEY-ASSIGN-MOBILE",
            FormatMessage = "Manual Trolley Assign Mobile",
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
            _transactionProducer.Publish<MobileManualTrolleyAssign>(message);
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
        var results = await _manualTrolleyAssignRepository.GetListDetailRequestAMR(requestNo);
        var grouped = results
        .GroupBy(x => new { x.RequestNo, x.TrolleyNo, x.IsCurrentProcessManual })
        .Select(g => new
        {
            g.Key.RequestNo,
            g.Key.TrolleyNo,
            g.Key.IsCurrentProcessManual,
            Details = g.Select(x => new
            {
                x.StopPoint,
                x.StopPointDesc,
                x.StatusAMR,
                x.IsCompleteLoading,
                x.IsCurrentProcessManual,
                x.LastUserRequestAMR,
                x.LastRequestDateAMR
            }).ToList()
        })
        .FirstOrDefault();

        return Success(grouped);
    }

    [HttpPost("send-cancel-request-amr")]
    public async Task<IActionResult> SendCancelRequestAMR([FromQuery] string requestNo)
    {
        var result = await _manualTrolleyAssignRepository.GetDataRequest(requestNo);

        if (String.IsNullOrEmpty(result.TrolleyNo)) return Invalid("Data Trolley belum memiliki trolley baru!");

        if (result.IsCurrentProcessManual) return Invalid("Data Request sudah manual!");

        await _manualTrolleyAssignRepository.SendRequestCancelAMR(result.RequestNo, result.TrolleyNo, Auth.User.UserID);

        BackgroundJob.Enqueue<IRobotService>(service => service.CancelRequest(result.RequestNo, result.TrolleyNo));

        return Success(message: "Requesting to Cancel Request AMR");
    }

    [HttpPost("send-complete-special-amr")]
    public async Task<IActionResult> SendCompleteSpecialAMR([FromQuery] string requestNo)
    {
        var result = await _manualTrolleyAssignRepository.GetDataRequest(requestNo);

        if (String.IsNullOrEmpty(result.LastStopPointComplete)) return Invalid("Belum ada Stop Point yang Complete Loading!");

        if (!result.IsCurrentProcessManual) return Invalid("Data Request sudah auto!");

        await _manualTrolleyAssignRepository.SendRequestCancelAMR(result.RequestNo, result.TrolleyNo, Auth.User.UserID);

        CompleteStatusRequest payload = new CompleteStatusRequest
        {
            RequestSendID = result.RequestNo,
            TrolleyNo = result.TrolleyNo,
            StopPoint = result.LastStopPointComplete,
            CompleteStatus = 1,
            IsManual = true
        };

        BackgroundJob.Enqueue<IRobotService>(service => service.CompleteLoadingSpecial(payload));

        return Success(message: "Requesting to Complete Special AMR");
    }
}
