using ClosedXML.Excel;
using Kawai.Api.Hub;
using Kawai.Api.Models;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Data;
using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/warehouse")]
[ApiController]
public class WarehouseController : HahaController
{
    private readonly IImportRepository _importRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly DataLogger _logger;

    public WarehouseController(IWarehouseRepository warehouseRepository, IImportRepository importRepository, DataLogger logger)
    {
        _warehouseRepository = warehouseRepository;
        _importRepository = importRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _warehouseRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string factoryCode, string ids)
    {
        var results = await _warehouseRepository.GetDDL(keyword, factoryCode);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("warehouseline-ddlsearch")]
    public async Task<IActionResult> DDLSearchWarehouseLine(string keyword, string factoryCode, string ids)
    {
        var results = await _warehouseRepository.GetDDLWarehouseLine(keyword, factoryCode);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-warehouse-search-by-stock")]
    public async Task<IActionResult> DDLSearchByStock(string keyword, string factoryCode, string item, string ids)
    {
        var results = await _warehouseRepository.DDLSearchByStock(keyword, factoryCode, item);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddlsearch-privileges")]
    public async Task<IActionResult> DDLPrivilegesSearch(string keyword, string factoryCode, string ids)
    {
        var results = await _warehouseRepository.GetDDLPrivileges(keyword, factoryCode, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddlsearch-subcon-privileges")]
    public async Task<IActionResult> DDLSubconPrivilegesSearch(string keyword, string factoryCode, string ids)
    {
        var results = await _warehouseRepository.GetDDLSubconPrivileges(keyword, factoryCode, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("warehouseline-ddlsearch-privileges")]
    public async Task<IActionResult> DDLPrivilegesSearchWarehouseLine(string keyword, string factoryCode, string ids)
    {
        var results = await _warehouseRepository.GetDDLPrivilegesWarehouseLine(keyword, factoryCode, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-warehouse-search-by-stock-privileges")]
    public async Task<IActionResult> DDLPrivilegesSearchByStock(string keyword, string factoryCode, string item, string ids)
    {
        var results = await _warehouseRepository.DDLPrivilegesSearchByStock(keyword, factoryCode, item, Auth.User.UserID);
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
        var result = await _warehouseRepository.GetData(id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Warehouse model)
    {
        await _warehouseRepository.Create(model, Auth.User.UserID);

        var after = await _warehouseRepository.Capture(model.WarehouseCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Warehouse",
            EntityId = model.WarehouseCode,
            ReferenceId = model.WarehouseCode,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });
        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Warehouse model)
    {
        var before = await _warehouseRepository.Capture(model.WarehouseCode);
        await _warehouseRepository.Update(model, Auth.User.UserID);
        var after = await _warehouseRepository.Capture(model.WarehouseCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Warehouse",
            EntityId = model.WarehouseCode,
            ReferenceId = model.WarehouseCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _warehouseRepository.Capture(id);
        await _warehouseRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Warehouse",
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
        var results = await _warehouseRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Warehouse Code", "Warehouse Name", "Adm Group", "Adm Group Name", "Stock Cls", "NG Cls", "Use End Date", "Last Update", "Last User"];
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
            ExcelHelper.SetCell(row, colIdx, result.AdmGroup);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.AdmGroupName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.StockControlCls == "01" ? "YES" : "NO");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.NGCls == "01" ? "YES" : "NO");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.UseEndDate.HasValue ? result.UseEndDate.Value.ToString("dd MMM yyyy") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Lastuser);
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

    [HttpPost("import")]
    public async Task<IActionResult> Import(ImportModel payload)
    {
        Stopwatch Timer = new();

        Timer.Start();

        // ambil data dari file & convert jadi List class import
        var list = ExcelHelper.ReadAndValidate<WarehouseImport>(payload.File);
        // kalo ada error dari hasil convert ke class
        var invalidRows = list.Where(x => !String.IsNullOrEmpty(x.Errors)).ToList();
        if (invalidRows.Any())
        {
            Timer.Stop();

            if (payload.Action == "EXECUTE")
                SaveImportHistory(payload, Timer, list, "FAILED");

            return ImportInvalid("DATA IMPORT TIDAK VALID", list);
        }

        // ubah jadi datatable disini, biar ga berkali-kali.
        var dtTable = DataTableHelper.ToDataTable(list);

        // get data setelah validasi
        var resultAfter = await _warehouseRepository.ValidateImport(dtTable, Auth.User.UserID);
        // kalo ada error setelah validasi
        invalidRows = resultAfter.Where(x => !String.IsNullOrEmpty(x.Errors)).ToList();
        if (invalidRows.Any())
        {
            Timer.Stop();

            if (payload.Action == "EXECUTE")
                SaveImportHistory(payload, Timer, resultAfter, "FAILED");

            return ImportInvalid("DATA IMPORT TIDAK VALID", resultAfter);
        }

        // kalo aksi nya execute maka langsung ke table. kalo cuma testing jangan.
        if (payload.Action == "EXECUTE")
        {
            await _warehouseRepository.Import(dtTable, Auth.User.UserID);

            foreach (var item in list)
            {
                var after = await _warehouseRepository.Capture(item.WarehouseCode);

                await _logger.SaveDataLog(new DataLogDto
                {
                    DocumentType = "Master Warehouse",
                    EntityId = item.WarehouseCode,
                    ReferenceId = item.WarehouseCode,
                    Action = DataLogAction.Import,
                    Activity = "Import Warehouse",
                    Before = null,
                    After = after
                });
            }

            SaveImportHistory(payload, Timer, resultAfter, "SUCCESS");
        }

        Timer.Stop();

        return Success(list);
    }

    private void SaveImportHistory(ImportModel payload, Stopwatch timer, List<WarehouseImport> result, string status)
    {
        var history = new ImportHistory
        {
            Id = Guid.NewGuid().UniqueId(),
            Template = "WarehouseImport",
            UserId = Auth.User.UserID,
            FileName = payload.File.FileName,
            ContentType = payload.File.ContentType,
            SizeFile = payload.File.Length,
            RowsCount = result.Count,
            ValidRowsCount = result.Where(p => String.IsNullOrEmpty(p.Errors)).Count(),
            InvalidRowsCount = result.Where(p => !String.IsNullOrEmpty(p.Errors)).Count(),
            Key = Guid.NewGuid().UniqueId(100),
            Status = status,
            ProcessDuration = timer.ElapsedMilliseconds
        };

        _importRepository.SaveHistory(history);
        FileStorage.SaveToImports(history.Id, payload.File);
    }
}
