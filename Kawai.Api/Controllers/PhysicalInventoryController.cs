using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
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

    [HttpPatch("approve")]
    public async Task<IActionResult> Approve([FromBody] List<PhysicalInventoryApproveDto> model)
    {

        var before = await _physicalInventoryRepository.CaptureApprove(model);
        await _physicalInventoryRepository.Approve(model, Auth.User.UserID);
        var after = await _physicalInventoryRepository.CaptureApprove(model);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Physical Inventory Approve",
            EntityId = Auth.User.UserID,
            ReferenceId = Auth.User.UserID,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
 
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _physicalInventoryRepository.GetList(parameter);
        if (results == null || !results.Any()) return NoContent();

        var resultheader = results.FirstOrDefault();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        // =========================
        // REPORT HEADER
        // =========================

        // Judul report
        ws.Cell(rowIdx, 1).Value = "PHYSICAL INVENTORY REPORT";
        ws.Range(rowIdx, 1, rowIdx, 14).Merge();
        ws.Cell(rowIdx, 1).Style.Font.Bold = true;
        ws.Cell(rowIdx, 1).Style.Font.FontSize = 16;
        ws.Cell(rowIdx, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(rowIdx, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        ws.Row(rowIdx).Height = 30;

        ws.Range(rowIdx, 1, rowIdx, 14).Style.Fill.BackgroundColor = XLColor.NoColor;
        ws.Range(rowIdx, 1, rowIdx, 14).Style.Font.FontColor = XLColor.Black;

        rowIdx++;

      
        ws.Cell(rowIdx, 1).Value = "Period";
        ws.Cell(rowIdx, 2).Value = resultheader.ParamPeriod;
        ws.Cell(rowIdx, 1).Style.Font.Bold = true;

        ws.Cell(rowIdx, 5).Value = "Item";
        ws.Cell(rowIdx, 6).Value = resultheader.ParamItem;
        ws.Cell(rowIdx, 5).Style.Font.Bold = true;

        rowIdx++;
        
        ws.Cell(rowIdx, 1).Value = "Warehouse";
        ws.Cell(rowIdx, 2).Value = resultheader.ParamWarehouse;
        ws.Cell(rowIdx, 1).Style.Font.Bold = true;

        ws.Cell(rowIdx, 5).Value = "Lot No";
        ws.Cell(rowIdx, 6).Value = resultheader.ParamLot;
        ws.Cell(rowIdx, 5).Style.Font.Bold = true;

        rowIdx++;
     
        ws.Cell(rowIdx, 1).Value = "Area";
        ws.Cell(rowIdx, 2).Value = resultheader.ParamArea;
        ws.Cell(rowIdx, 1).Style.Font.Bold = true;

        ws.Cell(rowIdx, 5).Value = "Status Scan";
        ws.Cell(rowIdx, 6).Value = resultheader.ParamScanStatus;
        ws.Cell(rowIdx, 5).Style.Font.Bold = true;

        rowIdx++;

        ws.Cell(rowIdx, 1).Value = "Address";
        ws.Cell(rowIdx, 2).Value = resultheader.ParamAddress;
        ws.Cell(rowIdx, 1).Style.Font.Bold = true;

        ws.Cell(rowIdx, 5).Value = "Different Qty";
        ws.Cell(rowIdx, 6).Value = resultheader.ParamDiffQty;
        ws.Cell(rowIdx, 5).Style.Font.Bold = true;
        rowIdx++;

        // Baris kosong sebagai pemisah
        rowIdx++;
     
        List<string> headers = ["Warehouse", "Area", "Address", "Barcode No", "Item Code", "Item Name", "Serial No", "Unit", "Lot No", "CurrentQty", "Inventory", "Difference", "LastUpdate", "LastUser"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.WarehouseName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AreaCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AddressCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BarcodeNo);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemDesc);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SerialNo);
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
        // Border hanya untuk header tabel mulai baris 7 sampai data terakhir
        var lastRow = ws.LastRowUsed().RowNumber();

        ws.Range(7, 1, lastRow, 14).Style.Border.OutsideBorder
            = XLBorderStyleValues.Thin;

        ws.Range(7, 1, lastRow, 14).Style.Border.InsideBorder
            = XLBorderStyleValues.Thin;
        ws.Columns().AdjustToContents();
        // =========================
        // SHEET 2: SUMMARY
        // =========================

        var wsSummary = workbook.Worksheets.Add("Summary");

        // Judul summary
        wsSummary.Cell(1, 1).Value = "PHYSICAL INVENTORY SUMMARY";
        wsSummary.Range(1, 1, 1, 3).Merge();

        wsSummary.Cell(1, 1).Style.Font.Bold = true;
        wsSummary.Cell(1, 1).Style.Font.FontSize = 16;
        wsSummary.Cell(1, 1).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        wsSummary.Cell(1, 1).Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        wsSummary.Row(1).Height = 30;

        // Header summary
        var summaryHeaders = new List<string>
    {
        "Item Code",
        "Item Name",
        "Total Qty"
    };

        ExcelHelper.SetHeader(wsSummary, 3, summaryHeaders);

        // Group berdasarkan ItemCode dan ItemDesc
        var summaryData = results
            .GroupBy(x => new
            {
                x.ItemCode,
                x.ItemDesc
            })
            .Select(g => new
            {
                ItemCode = g.Key.ItemCode,
                ItemName = g.Key.ItemDesc,
                TotalQty = g.Sum(x => x.CurrentQty)
            })
            .OrderBy(x => x.ItemCode)
            .ToList();

        // Isi data summary
        int summaryRow = 3;

        foreach (var item in summaryData)
        {
            summaryRow++;

            var row = wsSummary.Row(summaryRow);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx++, item.ItemCode);
            ExcelHelper.SetCell(row, colIdx++, item.ItemName);
            ExcelHelper.SetCell(row, colIdx++, item.TotalQty);
        }

        // Border summary
        var summaryLastRow = wsSummary.LastRowUsed().RowNumber();

        wsSummary.Range(3, 1, summaryLastRow, 3)
            .Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        wsSummary.Range(3, 1, summaryLastRow, 3)
            .Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Format kolom summary
        wsSummary.Columns().AdjustToContents();

        // Freeze header summary
        wsSummary.SheetView.FreezeRows(3);

        // =========================
        // EXPORT EXCEL
        // =========================

        using var ms = new MemoryStream();

        workbook.SaveAs(ms);

        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }
}
