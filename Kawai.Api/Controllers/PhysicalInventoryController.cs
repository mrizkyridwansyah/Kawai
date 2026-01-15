using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/physical-inventory")]
[ApiController]
public class PhysicalInventoryController : HahaController
{
    private readonly IPhysicalInventoryRepository _physicalInventoryRepository;
    private readonly DataLogger _logger;
    public PhysicalInventoryController(IPhysicalInventoryRepository physicalInventoryRepository, DataLogger logger)
    {
        _physicalInventoryRepository = physicalInventoryRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _physicalInventoryRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] List<PhysicalInventoryUpdateDto> model)
    {
        var before = await _physicalInventoryRepository.Capture(model);

        await _physicalInventoryRepository.Update(model, Auth.User.UserID);

        var after = await _physicalInventoryRepository.Capture(model);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Physical Inventory",
            EntityId = Auth.User.UserID,
            ReferenceId = Auth.User.UserID,
            Before = before,
            After = after,
            Activity = "Upsert Physical Inventory",
            Action = DataLogAction.Update
        });

        return Success(model);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _physicalInventoryRepository.GetList(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Warehouse", "Area", "Address", "Barcode No", "Item Code", "Item Name", "Unit", "Lot No", "CurrentQty", "Inventory", "Difference", "LastUpdate", "LastUser"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.WarehouseName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AreaName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AddressName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BarcodeNo);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Unit);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LotNo);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.CurrentQty);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Inventory);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.CurrentQty - result.Inventory);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUpdate?.ToString("dd MMM yyyy") ?? "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUserName);

            // warna background berdasarkan StatusScan
            if (result.StatusScan == "DIFFERENT")
            {
                ws.Range(rowIdx, 1, rowIdx, 9).Style.Fill.BackgroundColor = XLColor.Yellow;
            }
            else if (result.StatusScan == "SCANNED")
            {
                ws.Range(rowIdx, 1, rowIdx, 9).Style.Fill.BackgroundColor = XLColor.Gray;
            }
            else if (result.StatusScan == "NOTYET")
            {
                ws.Range(rowIdx, 1, rowIdx, 9).Style.Fill.BackgroundColor = XLColor.LightCoral;
            }

            // warna background kolom Difference
            ws.Range(rowIdx, 11, rowIdx, 11).Style.Fill.BackgroundColor = XLColor.LightYellow;
        }

        // =========================
        // FINAL TOUCH
        // =========================
        ws.Columns().AdjustToContents();
        ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }
}
