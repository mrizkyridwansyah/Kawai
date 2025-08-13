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
[Route("api/address")]
[ApiController]
public class AddressController : HahaController
{
    private readonly IAddressRepository _addressRepository;
    private readonly DataLogger _logger;

    public AddressController(IAddressRepository addressRepository, DataLogger logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _addressRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string warehouse, string area, string ids)
    {
        var results = await _addressRepository.GetDDL(keyword, warehouse, area);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.AddressCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-address-search-by-stock")]
    public async Task<IActionResult> DDLSearchByStock(string keyword, string warehouse, string area, string item, string ids)
    {
        var results = await _addressRepository.DDLSearchByStock(keyword, warehouse, area, item);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.AddressCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _addressRepository.GetData(id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Address model)
    {
        await _addressRepository.Create(model, Auth.User.UserID);

        var after = await _addressRepository.Capture(model.AddressCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Address",
            EntityId = model.AddressCode,
            ReferenceId = model.AddressCode,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Address model)
    {
        var before = await _addressRepository.Capture(model.AddressCode);
        await _addressRepository.Update(model, Auth.User.UserID);
        var after = await _addressRepository.Capture(model.AddressCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Address",
            EntityId = model.AddressCode,
            ReferenceId = model.AddressCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _addressRepository.Capture(id);
        await _addressRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Address",
            EntityId = id,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _addressRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Warehouse Code", "Warehouse Name", "Area Code", "Area Name",
                "Address Code", "Address Name", "Last Update", "Last User"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.WarehouseCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.WarehouseName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AreaCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AreaName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AddressCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AddressName);
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

    [HttpPost("export/qrcode")]
    public async Task<IActionResult> ExportQRCode([FromBody] List<Dictionary<string, object>> rows)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 2;
        int qrSize = 150;

        // Style for value cells
        var style = workbook.Style;
        style.Font.Bold = true;
        style.Font.FontSize = 16;
        style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        foreach (var rowMap in rows)
        {
            int startRow = rowIdx;
            var row = ws.Row(rowIdx);

            int colIdx = 2;

            // Insert QR code in cell
            string key = rowMap.TryGetValue("Key", out var keyVal) ? keyVal?.ToString() ?? "" : "";
            ExcelHelper.InsertQRCode(ws, rowIdx, colIdx++, key, qrSize);

            // Insert value cell with style
            string value = rowMap.TryGetValue("Value", out var val) ? val?.ToString() ?? "" : "";
            var valueCell = row.Cell(colIdx++);
            valueCell.Value = value;
            valueCell.Style = style;
            ws.Column(3).Width = 40;

            // Set outer border for range with QR + value
            var range = ws.Range(startRow, 2, rowIdx, colIdx - 1);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            rowIdx += 2;
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }
}
