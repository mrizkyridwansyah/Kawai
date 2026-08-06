using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models.Robot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/requestothers")]
[ApiController]
public class MobileRequestOthersController : HahaController
{
    private readonly IMobileRequestOthersRepository _requestothersRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;
    public MobileRequestOthersController(IMobileRequestOthersRepository requestothersRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _requestothersRepository = requestothersRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

    [HttpGet("ddlrequestno")]
    public async Task<IActionResult> DDLSearch(string keyword, string linecode, string requestno)
    {
        var results = await _requestothersRepository.GetRequestNoDDL(keyword, linecode, requestno);
        if (!string.IsNullOrEmpty(requestno))
        {
            var idList = requestno.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.RequestNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo, string linecode, string requestno)
    {
        var result = await _requestothersRepository.GetDataBarcode(barcodeNo, linecode, requestno);
        return Success(result);
    }


    [HttpGet("process-ddlsearch")]
    public async Task<IActionResult> ProcessDDL(string keyword,  string ids)
    {
        var results = await _requestothersRepository.GetProcessDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ManufactureCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("line-ddlsearch")]
    public async Task<IActionResult> LineDDL(string keyword, string warehousecode, string ids)
    {
        var results = await _requestothersRepository.GetLineDDL(keyword, warehousecode);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("list-material")]
    public async Task<IActionResult> GetListDetailMaterial(string requestno, string linecode)
    {
        var result = await _requestothersRepository.GetListDetailMaterial(requestno, linecode);
        result = result.ToList();
        return Success(result);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> GetListDetail(string linecode, string requestno, string itemCode)
    {
        var result = await _requestothersRepository.GetListDetail(linecode, requestno, itemCode);
        result = result.ToList();
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileRequestOthersSubmit model)
    {
        var message = new StockTransactionMessage<MobileRequestOthersSubmit>
        {
            Token = Auth.Token,
            AuthUserId = Auth.User.UserID,
            BroadcastBaseOn = "TOKEN",
            TimeStamp = EpochDateTime.Now,
            TransactionType = "REQUEST-OTHERS-MOBILE",
            FormatMessage = "Request Others Mobile",
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
            _transactionProducer.Publish<MobileRequestOthersSubmit>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

   



}
