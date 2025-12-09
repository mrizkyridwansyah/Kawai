using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/assign-storage")]
[ApiController]
public class MobileAssignStorageController : HahaController
{
    private readonly IMobileAssignStorageRepository _assignStorageRepository;
    private readonly ITransactionProducer _transactionProducer;

    public MobileAssignStorageController(IMobileAssignStorageRepository assignStorageRepository, ITransactionProducer transactionProducer)
    {
        _assignStorageRepository = assignStorageRepository;
        _transactionProducer = transactionProducer;
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo)
    {
        var result = await _assignStorageRepository.GetDataBarcode(barcodeNo);
        var grouped = result
            .GroupBy(x => new { x.RefNo, x.WarehouseCode, x.WarehouseName, x.AreaCode, x.AreaName, x.AddressCode, x.AddressName })
            .Select(g => new
            {
                g.Key.RefNo,
                g.Key.WarehouseCode,
                g.Key.WarehouseName,
                g.Key.AreaCode,
                g.Key.AreaName,
                g.Key.AddressCode,
                g.Key.AddressName,
                Details = g.Select(x => new
                {
                    x.ItemCode,
                    x.ItemName,
                    x.BarcodeNo,
                    x.LotNo,
                    x.SublotNo,
                    Qty = x.CurrentQty
                }).ToList()
            })
            .ToList();

        return Success(grouped);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileAssignStorage model)
    {
        var message = new StockTransactionMessage<MobileAssignStorage>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "ASSIGN-STORAGE-MOBILE",
            FormatMessage = "Assign Storage Mobile",
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

        _transactionProducer.Publish<MobileAssignStorage>(message);
        return Pending(message);
    }
}
