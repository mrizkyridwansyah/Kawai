using ClosedXML.Excel;
using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Api.Shared.Extension;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/warehouse")]
[ApiController]
public class WarehouseController : HahaController
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly DataLogger _logger;
    private readonly NotificationService<NotifApprovalHub> _notificationService;

    public WarehouseController(IWarehouseRepository warehouseRepository, INotificationRepository notificationRepository, DataLogger logger, NotificationService<NotifApprovalHub> notificationService)
    {
        _warehouseRepository = warehouseRepository;
        _notificationRepository = notificationRepository;
        _logger = logger;
        _notificationService = notificationService;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _warehouseRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _warehouseRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WarehouseCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-warehouse-search-by-stock")]
    public async Task<IActionResult> DDLSearchByStock(string keyword, string item, string ids)
    {
        var results = await _warehouseRepository.DDLSearchByStock(keyword, item);
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


        /*
         *  INI CONTOH KALO MAU PAKE NOTIF SETELAH API BIKIN SESUATU

            List<string> receivers = ["ossas"];
            Notification notification = new Notification
            {
                Title = "Master Warehouse Created",
                Description = $"Warehouse {model.WarehouseCode} - {model.WarehouseName} has been created.",
                NotifType = "INFO",
                Priority = "LOW",
                Sender = Auth.User.UserID
            };

            foreach (var reciver in receivers)
            {
                notification.Receiver = reciver;
                await _notificationRepository.SaveNotification(notification);
            }

            await _notificationService.BroadCastOnlyTo(receivers, "NewNotification", new { Count = 1 });         
         */

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
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Warehouse Code", "Warehouse Name", "Adm Group", "Adm Group Name", "Stock Cls", "NG Cls", "Use End Date", "Last Update", "Last User"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        var results = await _warehouseRepository.GetAll(parameter);

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
            ExcelHelper.SetCell(row, colIdx, result.UseEndDate.ToString("dd MMM yyyy"));
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
}
