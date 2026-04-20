using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/supply-subcon")]
[ApiController]
public class MobileSupplySubconController : HahaController
{
    private readonly IMobileSupplySubconRepository _supplySubconRepository;
    private readonly ITransactionProducer _transactionProducer;
    public MobileSupplySubconController(IMobileSupplySubconRepository supplySubconRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _supplySubconRepository = supplySubconRepository;
        _transactionProducer = transactionProducer;
    }

    [HttpGet("ddl-requestno")]
    public async Task<IActionResult> DDLSearch(string keyword, string itemClass, string supplierCode, string requestno)
    {
        var results = await _supplySubconRepository.GetRequestNoDDL(keyword, itemClass, supplierCode);
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
        var result = await _supplySubconRepository.GetDataBarcode(barcodeNo, requestno, itemClass);
        return Success(result);
    }

    [HttpGet("list-material")]
    public async Task<IActionResult> GetListDetailMaterial(string requestno, string itemClass)
    {
        var result = await _supplySubconRepository.GetListDetailMaterial(requestno, itemClass);
        return Success(result);
    }

    [HttpGet("list-stock")]
    public async Task<IActionResult> GetListStock(string itemCode)
    {
        var result = await _supplySubconRepository.GetListStock(itemCode);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileSupplySubcon model)
    {
        var message = new StockTransactionMessage<MobileSupplySubcon>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "SUPPLY-SUBCON-MOBILE",
            FormatMessage = "Supply Subcon Mobile",
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
            _transactionProducer.Publish<MobileSupplySubcon>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

}
