using ClosedXML.Excel;
using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/item")]
[ApiController]
public class ItemController : HahaController
{
    private readonly IItemRepository _itemRepository;
    private readonly DataLogger _logger;

    public ItemController(IItemRepository itemRepository, DataLogger logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _itemRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _itemRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ItemCode)).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpGet("ddl-item-search-by-stock")]
    public async Task<IActionResult> DDLItemSearchByStock(string keyword, string ids, string warehouse, string area, string address, string category, string statusReceipt, string statusHoldNG)
    {
        var results = await _itemRepository.DDLItemSearchByStock(keyword, warehouse, area, address, category, statusReceipt, statusHoldNG);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ItemCode)).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpGet("warehouse-ddlsearch")]
    public async Task<IActionResult> WarehouseDDLSearch(string keyword, string ids)
    {
        var results = await _itemRepository.GetWarehouseDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _itemRepository.GetData(id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Item model)
    {
        await _itemRepository.Create(model, Auth.User.UserID);

        var after = await _itemRepository.Capture(model.ItemCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Item",
            EntityId = model.ItemCode,
            ReferenceId = model.ItemCode,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Item model)
    {
        var before = await _itemRepository.Capture(model.ItemCode);
        await _itemRepository.Update(model, Auth.User.UserID);
        var after = await _itemRepository.Capture(model.ItemCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Item",
            EntityId = model.ItemCode,
            ReferenceId = model.ItemCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _itemRepository.Capture(id);
        await _itemRepository.Remove(id);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Item",
            EntityId = id,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }

    [HttpPost("export/magerexcel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _itemRepository.GetAll(parameter);

        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.AddWorksheet("Sheet1");
            MagerExcel mae = new MagerExcel(ws, 25.0);
            MagerExcel.CellResult cell = new MagerExcel.CellResult();

            List<object> headers =
            [
                "Item Code", "Item Name",
                "Warehouse Code", "Warehouse Name",
                "Supplier Code", "Supplier Name",
                "Manufacture Code", "Manufacture Name",
                "Finish Good Part", "Part Cls", "Reserve Cls", "Supply Cls", "Provision Cls", "Material Cls", "Production Cls", "Packing Style Cls", "Unit Cls", "Stock Control Cls",
                "Use End Date", "Last Update", "Last User"
            ];

            mae.DrawListRight(1, 1, headers, 1, 1, MagerExcel.BorderType.BorderAllThin, XLAlignmentVerticalValues.Center, XLAlignmentHorizontalValues.Center);

            cell.Row = 2;
            foreach (var result in results)
            {
                mae.DrawObject(cell.Row, 1, result.ItemCode);
                mae.DrawObject(cell.Row, 2, result.ItemName);
                mae.DrawObject(cell.Row, 3, result.WarehouseCode);
                mae.DrawObject(cell.Row, 4, result.WarehouseName);
                mae.DrawObject(cell.Row, 5, result.SupplierCode);
                mae.DrawObject(cell.Row, 6, result.SupplierName);
                mae.DrawObject(cell.Row, 7, result.ManufactureCode);
                mae.DrawObject(cell.Row, 8, result.ManufactureName);
                mae.DrawObject(cell.Row, 9, result.FinishGoodPartClsDesc);
                mae.DrawObject(cell.Row, 10, result.PartClsDesc);
                mae.DrawObject(cell.Row, 11, result.ReserveClsDesc);
                mae.DrawObject(cell.Row, 12, result.SupplyClsDesc);
                mae.DrawObject(cell.Row, 13, result.ProvisionClsDesc);
                mae.DrawObject(cell.Row, 14, result.MaterialClsDesc);
                mae.DrawObject(cell.Row, 15, result.ProductionClsDesc);
                mae.DrawObject(cell.Row, 16, result.PackingStyleClsDesc);
                mae.DrawObject(cell.Row, 17, result.UnitClsDesc);
                mae.DrawObject(cell.Row, 18, result.StockControlClsDesc);
                mae.DrawObject(cell.Row, 19, result.UseEndDay.HasValue ? result.UseEndDay.Value.ToString("dd MMM yyyy") : "");
                mae.DrawObject(cell.Row, 20, result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
                mae.DrawObject(cell.Row, 21, result.LastUser);

                cell.Row++;
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            var fileBytes = ms.ToArray();
            var base64File = Convert.ToBase64String(fileBytes);

            return Success(base64File);
        }

    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcelOld([FromBody] RequestParameter parameter)
    {
        var results = await _itemRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers =
        [
            "Item Code", "Item Name",
            "Warehouse Code", "Warehouse Name",
            "Supplier Code", "Supplier Name",
            "Manufacture Code", "Manufacture Name",
            "Finish Good Part", "Part Cls", "Reserve Cls", "Supply Cls", "Provision Cls", "Material Cls", "Production Cls", "Packing Style Cls", "Unit Cls", "Stock Control Cls",
            "Use End Date", "Last Update", "Last User"
        ];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        ws.Cell(2, 1).InsertData(results.Select(r => new
        {
            r.ItemCode,
            r.ItemName,
            r.WarehouseCode,
            r.WarehouseName,
            r.SupplierCode,
            r.SupplierName,
            r.ManufactureCode,
            r.ManufactureName,
            r.FinishGoodPartClsDesc,
            r.PartClsDesc,
            r.ReserveClsDesc,
            r.SupplyClsDesc,
            r.ProvisionClsDesc,
            r.MaterialClsDesc,
            r.ProductionClsDesc,
            r.PackingStyleClsDesc,
            r.UnitClsDesc,
            r.StockControlClsDesc,
            UseEndDay = r.UseEndDay.HasValue ? r.UseEndDay.Value.ToString("dd MMM yyyy") : "",
            LastUpdate = r.LastUpdate.HasValue ? r.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "",
            r.LastUser
        }));

        var range = ws.Range(1, 1, rowIdx, headers.Count);
        ExcelHelper.SetBorders(range);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms, false);
        ms.Position = 0;

        return File(ms.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "export.xlsx");
    }

    [HttpPost("export/excel-using-job")]
    public async Task<IActionResult> ExportExcelUsingJob(RequestParameter param)
    {
        string key = Guid.NewGuid().ToString();
        BackgroundJob.Enqueue<ExportService>(service => service.ExportExcelItem(param, Auth.Token, key));
        return Pending(message: "Data Export sedang diproses!");
    }
}
