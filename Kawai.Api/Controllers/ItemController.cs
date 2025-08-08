using ClosedXML.Excel;
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
public class IItemController : HahaController
{
    private readonly IItemRepository _itemRepository;
    private readonly DataLogger _logger;

    public IItemController(IItemRepository itemRepository, DataLogger logger)
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

        return Success(results);
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
        var before = await _itemRepository.Capture(model.WarehouseCode);
        await _itemRepository.Update(model, Auth.User.UserID);
        var after = await _itemRepository.Capture(model.WarehouseCode);

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
                mae.DrawObject(cell.Row, 1,  result.ItemCode);
                mae.DrawObject(cell.Row, 2,  result.ItemName);
                mae.DrawObject(cell.Row, 3,  result.WarehouseCode);
                mae.DrawObject(cell.Row, 4,  result.WarehouseName);
                mae.DrawObject(cell.Row, 5,  result.SupplierCode);
                mae.DrawObject(cell.Row, 6,  result.SupplierName);
                mae.DrawObject(cell.Row, 7,  result.ManufactureCode);
                mae.DrawObject(cell.Row, 8,  result.ManufactureName);
                mae.DrawObject(cell.Row, 9,  result.FinishGoodPartClsDesc);
                mae.DrawObject(cell.Row, 10,  result.PartClsDesc);
                mae.DrawObject(cell.Row, 11,  result.ReserveClsDesc);
                mae.DrawObject(cell.Row, 12,  result.SupplyClsDesc);
                mae.DrawObject(cell.Row, 13,  result.ProvisionClsDesc);
                mae.DrawObject(cell.Row, 14,  result.MaterialClsDesc);
                mae.DrawObject(cell.Row, 15,  result.ProductionClsDesc);
                mae.DrawObject(cell.Row, 16,  result.PackingStyleClsDesc);
                mae.DrawObject(cell.Row, 17,  result.UnitClsDesc);
                mae.DrawObject(cell.Row, 18,  result.StockControlClsDesc);
                mae.DrawObject(cell.Row, 19,  result.UseEndDay.HasValue ? result.UseEndDay.Value.ToString("dd MMM yyyy") : "");
                mae.DrawObject(cell.Row, 20,  result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
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

        var results = await _itemRepository.GetAll(parameter);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.ItemCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.WarehouseCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.WarehouseName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SupplierCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SupplierName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ManufactureCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ManufactureName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.FinishGoodPartClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PartClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ReserveClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SupplyClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ProvisionClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.MaterialClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ProductionClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PackingStyleClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.UnitClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.StockControlClsDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.UseEndDay.HasValue ? result.UseEndDay.Value.ToString("dd MMM yyyy") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUser);
        }

        ExcelHelper.AutofitColumns(ws, 1, headers.Count);

        var range = ws.Range(1, 1, rowIdx, headers.Count);
        ExcelHelper.SetBorders(range);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }
}
