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

        if (results == null || !results.Any())
            return NoContent();

        // =========================
        // FLAG DATA ALLOCATION
        // =========================

        string FlagDataAllocation = parameter.Filters[0]["DifferentData"].ToString();

        var resultheader = results.FirstOrDefault();

        using var workbook = new XLWorkbook();

        // =========================
        // FUNCTION CREATE SHEET DATA
        // =========================

        void CreateDataSheet(string sheetName, IEnumerable<dynamic> sheetResults)
        {
            var ws = workbook.Worksheets.Add(sheetName);

            int rowIdx = 1;

            // =========================
            // REPORT HEADER
            // =========================

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

            string DiffQty;
           
            if (resultheader.ParamDiffQty == "true")   
                {
                DiffQty = "YES";
                }
                else
                {
                    DiffQty = "NO";
                }

            ws.Cell(rowIdx, 5).Value = "Different Qty";
            ws.Cell(rowIdx, 6).Value = DiffQty;
            ws.Cell(rowIdx, 5).Style.Font.Bold = true;

            rowIdx++;

            // Baris kosong
            rowIdx++;

            List<string> headers =
            [
                "Warehouse", "Area", "Address", "Barcode No",
            "Item Code", "Item Name", "Serial No", "Unit",
            "Lot No", "CurrentQty", "Inventory", "Difference",
            "LastUpdate", "LastUser"
            ];

            ExcelHelper.SetHeader(ws, rowIdx, headers);

            // =========================
            // DATA
            // =========================

            foreach (var result in sheetResults)
            {
                rowIdx++;

                var row = ws.Row(rowIdx);
                int colIdx = 1;

                ExcelHelper.SetCell(row, colIdx++, result.WarehouseName);
                ExcelHelper.SetCell(row, colIdx++, result.AreaCode);
                ExcelHelper.SetCell(row, colIdx++, result.AddressCode);
                ExcelHelper.SetCell(row, colIdx++, result.BarcodeNo);
                ExcelHelper.SetCell(row, colIdx++, result.ItemCode);
                ExcelHelper.SetCell(row, colIdx++, result.ItemDesc);
                ExcelHelper.SetCell(row, colIdx++, result.SerialNo);
                ExcelHelper.SetCell(row, colIdx++, result.Unit);
                ExcelHelper.SetCell(row, colIdx++, result.LotNo);
                ExcelHelper.SetCell(row, colIdx++, result.CurrentQty);
                ExcelHelper.SetCell(row, colIdx++, result.Inventory);
                ExcelHelper.SetCell(row, colIdx++, result.CurrentQty - result.Inventory);
                ExcelHelper.SetCell(row, colIdx++, result.LastUpdate?.ToString("dd MMM yyyy") ?? "");
                ExcelHelper.SetCell(row, colIdx++, result.LastUserName);

                // Warna berdasarkan StatusScan
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

                // Warna kolom Difference
                ws.Range(rowIdx, 12, rowIdx, 12).Style.Fill.BackgroundColor = XLColor.LightYellow;
            }

            // =========================
            // FINAL TOUCH
            // =========================

            var lastRow = ws.LastRowUsed().RowNumber();

            ws.Range(7, 1, lastRow, 14).Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            ws.Range(7, 1, lastRow, 14).Style.Border.InsideBorder =
                XLBorderStyleValues.Thin;

            ws.Columns().AdjustToContents();
        }

        // =========================
        // SHEET 1: DATA
        // =========================

        if (FlagDataAllocation == "true")
        {
            // Data hanya PickingStatus = 0
            CreateDataSheet(
                "Data",
                results.Where(x =>  x.PickingStatus  == "0").Cast<dynamic>()
            );

            // Data Allocation hanya PickingStatus = 1
            CreateDataSheet(
                "Data Allocation Stock",
                results.Where(x =>  x.PickingStatus  == "1").Cast<dynamic>()
            );
        }
        else
        {
            // Kondisi normal: semua data
            CreateDataSheet(
                "Data",
                results.Cast<dynamic>()
            );
        }

        // =========================
        // SHEET SUMMARY
        // =========================

        var wsSummary = workbook.Worksheets.Add("Summary");

        bool isDataAllocation = FlagDataAllocation == "true";

        // Judul summary
        wsSummary.Cell(1, 1).Value = "PHYSICAL INVENTORY SUMMARY";

        // Merge judul menyesuaikan jumlah kolom
        wsSummary.Range(1, 1, 1, isDataAllocation ? 4 : 3).Merge();

        wsSummary.Cell(1, 1).Style.Font.Bold = true;
        wsSummary.Cell(1, 1).Style.Font.FontSize = 16;
        wsSummary.Cell(1, 1).Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;
        wsSummary.Cell(1, 1).Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        wsSummary.Row(1).Height = 30;

        // =========================
        // HEADER SUMMARY
        // =========================

        var summaryHeaders = new List<string>
        {
            "Item Code",
            "Item Name",
            "Total Qty"
        };

        // Jika FlagDataAllocation = 1
        // Tambahkan kolom Total Qty Allocation
        if (isDataAllocation)
        {
            summaryHeaders.Add("Total Qty Allocation");
        }

        ExcelHelper.SetHeader(wsSummary, 3, summaryHeaders);

        // =========================
        // GROUP SUMMARY DATA
        // =========================

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

                // Total Qty hanya PickingStatus = 0
                TotalQty = isDataAllocation
                    ? g.Where(x => x.PickingStatus == "0")
                       .Sum(x => x.CurrentQty)
                    : g.Sum(x => x.CurrentQty),

                // Total Qty Allocation hanya PickingStatus = 1
                TotalQtyAllocation = isDataAllocation
                    ? g.Where(x =>  x.PickingStatus  == "1")
                       .Sum(x => x.CurrentQty)
                    : 0
            })
            .OrderBy(x => x.ItemCode)
            .ToList();

        // =========================
        // ISI DATA SUMMARY
        // =========================

        int summaryRow = 3;

        foreach (var item in summaryData)
        {
            summaryRow++;

            var row = wsSummary.Row(summaryRow);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx++, item.ItemCode);
            ExcelHelper.SetCell(row, colIdx++, item.ItemName);
            ExcelHelper.SetCell(row, colIdx++, item.TotalQty);

            if (isDataAllocation)
            {
                ExcelHelper.SetCell(row, colIdx++, item.TotalQtyAllocation);
            }
        }

        // =========================
        // BORDER SUMMARY
        // =========================

        var summaryLastRow = wsSummary.LastRowUsed().RowNumber();

        int totalColumns = isDataAllocation ? 4 : 3;

        wsSummary.Range(3, 1, summaryLastRow, totalColumns)
            .Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        wsSummary.Range(3, 1, summaryLastRow, totalColumns)
            .Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        // Format kolom
        wsSummary.Columns().AdjustToContents();

        // Freeze header
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
