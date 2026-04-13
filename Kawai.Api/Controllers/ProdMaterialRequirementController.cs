using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/prod-material-requirement")]
[ApiController]
public class ProdMaterialRequirementController : HahaController
{
    private readonly IProdMaterialRequirementRepository _prodMaterialRequirement;
    private readonly DataLogger _logger;

    public ProdMaterialRequirementController(IProdMaterialRequirementRepository prodMaterialRequirementRepository, DataLogger logger)
    {
        _prodMaterialRequirement = prodMaterialRequirementRepository;
        _logger = logger;
    }

    [HttpGet("last-calculation")]
    public async Task<IActionResult> GetLastCalculation(string factory)
    {
        var headerTask = _prodMaterialRequirement.GetLastCalculation(factory);
        var detailsTask = _prodMaterialRequirement.GetListDetail(factory);

        await Task.WhenAll(headerTask, detailsTask);

        var result = new
        {
            Header = headerTask.Result,
            Materials = detailsTask.Result.GroupBy(x => new { x.ParamKey, x.ChildItemCode, x.ChildItemName, x.UnitCls, x.UnitClsName, x.TotalReqQty, x.CurrentStock, x.Shortage })
            .Select(g => new
            {
                g.Key.ParamKey,
                g.Key.ChildItemCode,
                g.Key.ChildItemName,
                g.Key.UnitCls,
                g.Key.UnitClsName,
                g.Key.TotalReqQty,
                g.Key.CurrentStock,
                g.Key.Shortage,
                Details = g.Where(p => p.FinalReqQty > 0).Select(x => new
                {
                    x.Line,
                    x.LineName,
                    x.ParentItemCode,
                    x.ParentItemName,
                    x.ScheduleDate,
                    x.FinalReqQty
                }).OrderBy(p => p.ScheduleDate).ThenBy(p => p.Line).ToList()
            }).OrderBy(p => p.ChildItemCode).ToList()
        };

        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] ProdMaterialRequirement model)
    {
        await _prodMaterialRequirement.Save(model, Auth.User.UserID);

        var after = await _prodMaterialRequirement.Capture(model.ParamKey);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Prod Material Requirement",
            EntityId = model.ParamKey,
            ReferenceId = model.ParamKey,
            After = after,
            Action = DataLogAction.Create,
            Activity = "Save Calculation Prod Material Requirement"
        });

        return Success();
    }

    [HttpGet("export/excel-inquiry")]
    public async Task<IActionResult> ExportExcel(string factory)
    {
        var headerTask = _prodMaterialRequirement.GetLastCalculation(factory);
        var detailsTask = _prodMaterialRequirement.GetListDetail(factory);

        await Task.WhenAll(headerTask, detailsTask);

        var result = new
        {
            Header = headerTask.Result,
            Materials = detailsTask.Result.GroupBy(x => new { x.ParamKey, x.ChildItemCode, x.ChildItemName, x.UnitCls, x.UnitClsName, x.TotalReqQty, x.CurrentStock, x.Shortage })
            .Select(g => new
            {
                g.Key.ParamKey,
                g.Key.ChildItemCode,
                g.Key.ChildItemName,
                g.Key.UnitCls,
                g.Key.UnitClsName,
                g.Key.TotalReqQty,
                g.Key.CurrentStock,
                g.Key.Shortage,
                Details = g.Where(p => p.FinalReqQty > 0).Select(x => new
                {
                    x.Line,
                    x.LineName,
                    x.ParentItemCode,
                    x.ParentItemName,
                    x.ScheduleDate,
                    x.FinalReqQty
                }).OrderBy(p => p.ScheduleDate).ThenBy(p => p.Line).ToList()
            }).OrderBy(p => p.ChildItemCode).ToList()
        };

        if (result == null || !result.Materials.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdxHeader = 1;

        var rowHeader = ws.Row(rowIdxHeader);
        ExcelHelper.SetCell(rowHeader, 7, "Last Calculation Time");
        ExcelHelper.SetCell(rowHeader, 8, ": " + result.Header.LastCalculation);
        ExcelHelper.MergeCells(ws, rowIdxHeader, rowIdxHeader, 8, 10);

        rowIdxHeader++;
        rowHeader = ws.Row(rowIdxHeader);
        ExcelHelper.SetCell(rowHeader, 7, "Factory");
        ExcelHelper.SetCell(rowHeader, 8, ": " + result.Header.FactoryName);
        ExcelHelper.MergeCells(ws, rowIdxHeader, rowIdxHeader, 8, 10);

        rowIdxHeader++;
        rowHeader = ws.Row(rowIdxHeader);
        ExcelHelper.SetCell(rowHeader, 7, "Process");
        ExcelHelper.SetCell(rowHeader, 8, ": " + result.Header.ProcessName);
        ExcelHelper.MergeCells(ws, rowIdxHeader, rowIdxHeader, 8, 10);

        rowIdxHeader++;
        rowHeader = ws.Row(rowIdxHeader);
        ExcelHelper.SetCell(rowHeader, 7, "Line");
        ExcelHelper.SetCell(rowHeader, 8, ": " + result.Header.LineName);
        ExcelHelper.MergeCells(ws, rowIdxHeader, rowIdxHeader, 8, 10);

        rowIdxHeader++;
        rowHeader = ws.Row(rowIdxHeader);
        ExcelHelper.SetCell(rowHeader, 7, "Model");
        ExcelHelper.SetCell(rowHeader, 8, ": " + result.Header.ModelName);
        ExcelHelper.MergeCells(ws, rowIdxHeader, rowIdxHeader, 8, 10);

        ExcelHelper.ApplyHeaderStyle(ws, 1, 5, 7, 10, border: XLBorderStyleValues.None, hAlign: XLAlignmentHorizontalValues.Left, fontColor: XLColor.Black);

        int rowIdx = 7;

        List<string> headers = ["Material Code", "Material Name", "Unit", "Request Qty", "Current Stock", "Shortage", "Line", "Schedule Date", "Parent Item", "Qty"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var material in result.Materials)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, material.ChildItemCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, material.ChildItemName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, material.UnitClsName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, material.TotalReqQty);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, material.CurrentStock);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, material.Shortage, cell =>
            {
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = material.Shortage < 0 ? XLColor.Salmon : XLColor.Green;
            });
            colIdx++;
            ExcelHelper.MergeCells(ws, rowIdx, rowIdx, colIdx, colIdx + 3);

            foreach (var detail in material.Details)
            {
                rowIdx++;
                var rowDetail = ws.Row(rowIdx);
                colIdx = 7;

                ExcelHelper.MergeCells(ws, rowIdx, rowIdx, 1, 6);
                ExcelHelper.SetCell(rowDetail, colIdx, detail.LineName);
                colIdx++;
                ExcelHelper.SetCell(rowDetail, colIdx, detail.ScheduleDate.ToString("dd MMM yyyy"));
                colIdx++;
                ExcelHelper.SetCell(rowDetail, colIdx, detail.ParentItemName);
                colIdx++;
                ExcelHelper.SetCell(rowDetail, colIdx, detail.FinalReqQty);
            }
        }

        ExcelHelper.AutofitColumns(ws, 1, headers.Count);

        var range = ws.Range(7, 1, rowIdx, headers.Count);
        ExcelHelper.SetBorders(range);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }

}
