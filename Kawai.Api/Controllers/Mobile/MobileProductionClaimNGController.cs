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
[Route("api/mobile/productionclaimng")]
[ApiController]
public class MobileProductionClaimNGController : HahaController
{
    private readonly IMobileProductionClaimNGRepository _productionclaimngRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;
    public MobileProductionClaimNGController(IMobileProductionClaimNGRepository productionclaimngRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _productionclaimngRepository = productionclaimngRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

    [HttpGet("ddlclaimno")]
    public async Task<IActionResult> DDLSearch(string keyword, string linecode, string claimno)
    {
        var results = await _productionclaimngRepository.GetClaimNoDDL(keyword, linecode, claimno);
        if (!string.IsNullOrEmpty(claimno))
        {
            var idList = claimno.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ClaimNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo, string linecode, string claimno)
    {
        var result = await _productionclaimngRepository.GetDataBarcode(barcodeNo, linecode, claimno);
        return Success(result);
    }


    [HttpGet("process-ddlsearch")]
    public async Task<IActionResult> ProcessDDL(string keyword,  string ids)
    {
        var results = await _productionclaimngRepository.GetProcessDDL(keyword);
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
        var results = await _productionclaimngRepository.GetLineDDL(keyword, warehousecode);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("list-material")]
    public async Task<IActionResult> GetListDetailMaterial(string claimno, string linecode)
    {
        var result = await _productionclaimngRepository.GetListDetailMaterial(claimno, linecode);
        result = result.ToList();
        return Success(result);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> GetListDetail(string linecode, string claimno, string pickingno, string itemCode)
    {
        var result = await _productionclaimngRepository.GetListDetail(linecode, claimno, pickingno, itemCode);
        result = result.ToList();
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobilProductionClaimNGSubmit model)
    {
        var message = new StockTransactionMessage<MobilProductionClaimNGSubmit>
        {
            Token = Auth.Token,
            AuthUserId = Auth.User.UserID,
            BroadcastBaseOn = "TOKEN",
            TimeStamp = EpochDateTime.Now,
            TransactionType = "PRODUCTION-CLAIM-NG-MOBILE",
            FormatMessage = "Production Claim NG Mobile",
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
            _transactionProducer.Publish<MobilProductionClaimNGSubmit>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

   



}
