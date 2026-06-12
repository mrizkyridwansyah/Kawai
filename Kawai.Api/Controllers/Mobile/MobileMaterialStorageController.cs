using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/material-storage")]
[ApiController]
public class MobileMaterialStorageController : HahaController
{
    private readonly IMobileMaterialStorageRepository _materialStorageRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;

    public MobileMaterialStorageController(IMobileMaterialStorageRepository materialStorageRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _materialStorageRepository = materialStorageRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

    [HttpGet("data-summary")]
    public async Task<IActionResult> GetSummaryStorage(string warehouseCode, string barcode)
    {
        var result = await _materialStorageRepository.GetSummaryStorage(warehouseCode, barcode);
        return Success(result);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> GetListDetail(string warehouseCode, string lotNo, string itemCode)
    {
        var result = await _materialStorageRepository.GetListDetail(warehouseCode, lotNo, itemCode);
        return Success(result);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo, bool onlyTemp)
    {
        var result = await _materialStorageRepository.GetDataBarcode(barcodeNo, onlyTemp);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileMaterialStorage model)
    {
        var message = new StockTransactionMessage<MobileMaterialStorage>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "MATERIAL-STORAGE-MOBILE",
            FormatMessage = "Material Storage Mobile",
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
            _transactionProducer.Publish<MobileMaterialStorage>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpPost("save-merge")]
    public async Task<IActionResult> SaveMerge(MobileMaterialMergeStorage model)
    {
        var message = new StockTransactionMessage<MobileMaterialMergeStorage>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "MATERIAL-MERGE-STORAGE-MOBILE",
            FormatMessage = "Material Merge Storage Mobile",
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
            _transactionProducer.Publish<MobileMaterialMergeStorage>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpGet("list-remaining-stock")]
    public async Task<IActionResult> GetListRemainingStock()
    {
        var result = await _materialStorageRepository.GetListRemainingStock(Auth.User.UserID);
        return Success(result);
    }
}
