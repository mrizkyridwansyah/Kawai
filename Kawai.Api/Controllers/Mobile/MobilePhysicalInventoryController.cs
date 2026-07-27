using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;
using Kawai.Domain.DTOs.Log;
using Microsoft.AspNetCore.Authorization;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/physical-inventory")]
[ApiController]
public class MobilePhysicalInventoryController : HahaController
{
    private readonly IMobilePhysicalInventoryRepository _mobilePhysicalInventoryRepository;
    private readonly DataLogger _logger;

    public MobilePhysicalInventoryController(IMobilePhysicalInventoryRepository mobilePhysicalInventoryRepository, DataLogger logger)
    {
        _mobilePhysicalInventoryRepository = mobilePhysicalInventoryRepository;
        _logger = logger;
    }

    [HttpGet("list-stock")]
    public async Task<IActionResult> GetListStock(string addressCode)
    {
        var result = await _mobilePhysicalInventoryRepository.GetListStock(addressCode, Auth.User.UserID);
        var grouped = result
        .GroupBy(x => new { x.AddressCode, x.AddressName })
        .Select(g => new
        {
            g.Key.AddressCode,
            g.Key.AddressName,
            Details = g.Select(x => new
            {
                x.ItemCode,
                x.ItemName,
                x.BarcodeNo,
                x.LotNo,
                x.SublotNo,
                x.CurrentQty,
                x.InventoryQty,
                x.StatusSO
            }).ToList()
        })
        .ToList();
        return Success(grouped);
    }

    [HttpGet("list-stock-summary")]
    public async Task<IActionResult> GetListStockSummary(string addressCode)
    {
        var results = await _mobilePhysicalInventoryRepository.GetListStockSummary(addressCode);
        return Success(results);
    }

    [HttpGet("list-stock-detail")]
    public async Task<IActionResult> GetListStockDetail(string addressCode, string itemCode)
    {
        var results = await _mobilePhysicalInventoryRepository.GetListStockDetail(addressCode, itemCode);
        return Success(results);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string addressCode, string barcodeNo)
    {
        var result = await _mobilePhysicalInventoryRepository.GetDataBarcode(addressCode, barcodeNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobilePhysicalInventory model)
    {
        var before = await _mobilePhysicalInventoryRepository.Capture(model.BarcodeNo);

        await _mobilePhysicalInventoryRepository.Save(model, Auth.User.UserID);

        var after = await _mobilePhysicalInventoryRepository.Capture(model.BarcodeNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Physical Inventory",
            EntityId = model.BarcodeNo.ToString(),
            ReferenceId = model.BarcodeNo.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Physical Inventory",
            Action = DataLogAction.Update
        });
        return Success(after);

        //var message = new StockTransactionMessage<MobileAssignStorage>
        //{
        //    AuthUserId = Auth.User.UserID,
        //    TimeStamp = EpochDateTime.Now,
        //    TransactionType = "ASSIGN-STORAGE-MOBILE",
        //    FormatMessage = "Assign Storage Mobile",
        //    Payload = model,
        //    LogContext = new LogContext
        //    {
        //        Method = HttpContext.Request.Method,
        //        RequestPath = HttpContext.Request.Path,
        //        RemoteAddr = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
        //        UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
        //        UserID = Auth.User.UserID,
        //        FullName = Auth.User.FullName
        //    }
        //};

        //_transactionProducer.Publish<MobileAssignStorage>(message);
        //return Pending(message);
    }

    [HttpGet("by-warehouse/data-barcode")]
    public async Task<IActionResult> GetDataBarcodeByWarehouse(string warehouseCode, string barcodeNo)
    {
        var result = await _mobilePhysicalInventoryRepository.GetDataBarcodeByWarehouse(warehouseCode, barcodeNo);
        return Success(result);
    }

    [HttpPost("by-warehouse/save")]
    public async Task<IActionResult> SaveByWarehouse(MobilePhysicalInventoryWarehouse model)
    {
        var before = await _mobilePhysicalInventoryRepository.Capture(model.BarcodeNo);

        await _mobilePhysicalInventoryRepository.SaveByWarehouse(model, Auth.User.UserID);

        var after = await _mobilePhysicalInventoryRepository.Capture(model.BarcodeNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Physical Inventory",
            EntityId = model.BarcodeNo.ToString(),
            ReferenceId = model.BarcodeNo.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Physical Inventory",
            Action = DataLogAction.Update
        });
        return Success(after);
    }
}