using ClosedXML.Excel;
using Hangfire;
using Kawai.Api.Hub;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Kawai.Api.Services;

/* 
 * Tambahin dulu di interface nya mau export apa aja yg di handle pake background job ini
 */
public interface IExportService
{
    Task ExportExcelItem(RequestParameter param, string userId, string key);
    Task ExportExcelReceiptInquiry(RequestParameter param, string userId, string key);
    Task ExportPdfReceiptBarcode(ReceiptDto receipt, string userId);
    Task ExportPdfIQCReportNG(List<QualityCheckReportDto> list, string userId, string key);
    Task ExportWomin(long requestId, string userId, string key);
    Task ExportPdfReprintBarcode(List<SelectedPrintDto> selectedPrint, string userId, string key);
}

public class ExportService : IExportService
{
    private readonly RazorViewRenderer _renderer;
    private readonly IFileStorage _fileStorage;
    private readonly IItemRepository _itemRepository;
    private readonly IPartMaterialRequestWominRepository _wominRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IReprintRepository _reprintRepository;
    private readonly NotificationService<NotifApprovalHub> _notificationService;
    private readonly DbExecutor _dbExecutor;

    public ExportService
    (
        IFileStorage fileStorage,
        IItemRepository itemRepository,
        IReceiptRepository receiptRepository,
        IReprintRepository reprintRepository,
        IPartMaterialRequestWominRepository wominRepository,
        NotificationService<NotifApprovalHub> notificationService,
        RazorViewRenderer renderer,
        DbExecutor dbExecutor
    )
    {
        _fileStorage = fileStorage;
        _itemRepository = itemRepository;
        _receiptRepository = receiptRepository;
        _reprintRepository = reprintRepository;
        _wominRepository = wominRepository;
        _notificationService = notificationService;
        _renderer = renderer;
        _dbExecutor = dbExecutor;
    }

    /* 
     * Ini contoh penerapannya.
     * File akan digenerate lalu disimpan ke directory
     * Setelah itu SignalR akan broadcast ke user yg tadi export kalo file nya udah siap dengan mengirikan Key (ID dari file di direktori)+ FileName (buat jadi nama setelah didownload)
     */
    [AutomaticRetry(Attempts = 0)]
    public async Task ExportExcelItem(RequestParameter param, string userId, string key)
    {
        var results = await _itemRepository.GetAll(param);
        //if (results == null || !results.Any()) return NoContent();

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
        ExcelHelper.AutofitColumns(ws, 1, headers.Count);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms, false);
        ms.Position = 0;

        _fileStorage.SaveToExports(key, ms);

        int defaultTTLMinute = 5;// simpen file fisik nya selama 5 menit

        // Masukkan ke Table ExportFile kalo file hasil export nya mau di hapus
        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO ExportFile (FileKey, RegisterDate, TTLMinute)
            VALUES (@key, GETDATE(), @ttl)", new { key, ttl = defaultTTLMinute }, commandType: CommandType.Text);

        await _notificationService.BroadCastOnlyTo([userId], "FileExportExcel", new { KeyFile = key, KeyStorage = key, FileName = "Master Item" });
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task ExportWomin(long requestId, string userId, string key)
    {
        var results = await _wominRepository.WominReport(requestId);
        //if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers =
        [
            "Production Date","Line Code","Line Name","Parent Item Code","Parent Item Name","Request Qty","WorkStation Code","Area","Ref Number","Child Item Code","Child Item Name","Child Requirement Qty","Child Scan Qty"
        ];

        ExcelHelper.SetHeader(ws, rowIdx, headers);

        ws.Cell(2, 1).InsertData(results.Select(r => new
        {
            r.ProductionDate, 
            r.LineCode,
            r.LineName,
            r.ParentItemCode,
            r.ParentItemName,
            r.RequestSetQty,
            r.WorkStationCode,
            r.Area,
            r.RefNumber,
            r.ChildItemCode,
            r.ChildItemName,
            r.ChildRequirementQty,
            r.ChildScanQty
 
        }));

        var range = ws.Range(1, 1, rowIdx, headers.Count);
        ExcelHelper.SetBorders(range);
        ExcelHelper.AutofitColumns(ws, 1, headers.Count);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms, false);
        ms.Position = 0;

        _fileStorage.SaveToExports(key, ms);

        int defaultTTLMinute = 0;// simpen file fisik nya selama 5 menit

        // Masukkan ke Table ExportFile kalo file hasil export nya mau di hapus
        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO ExportFile (FileKey, RegisterDate, TTLMinute)
            VALUES (@key, GETDATE(), @ttl)", new { key, ttl = defaultTTLMinute }, commandType: CommandType.Text);

        await _notificationService.BroadCastOnlyTo([userId], "FileExportExcel", new { KeyFile = key, KeyStorage = key, FileName = "Data Womin" });
    }



    [AutomaticRetry(Attempts = 0)]
    public async Task ExportPdfReceiptBarcode(ReceiptDto receipt, string userId)
    {
        var results = await _receiptRepository.GetListBarcodeDetail(receipt.Id.Value);
        var models = results.Select(item => new LabelBarcodeDetailDto
        {
            BarcodeNo = item.BarcodeNo,
            ReceiptNo = item.ReceiptNo,
            FromCompany = item.FromCompany,
            ToCompany = item.ToCompany,
            PONumber = item.PONumber,
            ShippingLot = item.ShippingLot,
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            Qty = item.Qty,
            DeliveryDate = item.DeliveryDate,
            DNNumber = item.DNNumber,
            ShippingLabelNo = item.ShippingLabelNo,
        }).ToList();

        var fullHtml = await _renderer.RenderAsync("Templates/PrintBarcodesA4.cshtml", models);
        var pdfBytes = await _renderer.GeneratePdfAsync(fullHtml);

        string keyStorage = Guid.NewGuid().ToString();
        string key = "PrintBarcodeUsingJob_" + receipt.Id.Value.ToString() + "_" + keyStorage;
        _fileStorage.SaveToExports(key, new MemoryStream(pdfBytes));


        int defaultTTLMinute = 5;// simpen file fisik nya selama 1 bulan. 

        // Masukkan ke Table ExportFile kalo file hasil export nya mau di hapus
        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO ExportFile (FileKey, RegisterDate, TTLMinute)
            VALUES (@key, GETDATE(), @ttl)", new { key, ttl = defaultTTLMinute }, commandType: CommandType.Text);

        await _notificationService.BroadCastOnlyTo([userId], "FileExportPDF", new { KeyFile = key, KeyStorage = keyStorage, FileName = receipt.SupplierName + "_" + receipt.DNNumber });
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task ExportPdfIQCReportNG(List<QualityCheckReportDto> list, string userId, string key)
    {
        var fullHtml = await _renderer.RenderAsync(
            "Templates/QCReport.cshtml",
            list);

        var pdfBytes = await _renderer.GeneratePdfAsync(fullHtml);

        _fileStorage.SaveToExports(key, new MemoryStream(pdfBytes));

        string keyStorage = Guid.NewGuid().ToString();

        int defaultTTLMinute = 5;// simpen file fisik nya selama 5 menit aja. 

        // Masukkan ke Table ExportFile kalo file hasil export nya mau di hapus
        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO ExportFile (FileKey, RegisterDate, TTLMinute)
            VALUES (@key, GETDATE(), @ttl)", new { key, ttl = defaultTTLMinute }, commandType: CommandType.Text);

        await _notificationService.BroadCastOnlyTo([userId], "FileExportPDF", new { KeyFile = key, KeyStorage = keyStorage, FileName = "Report_NG_" + list[0].DNNumber });
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task ExportExcelReceiptInquiry(RequestParameter param, string userId, string key)
    {
        var results = await _receiptRepository.Inquiry(param);
        if (results == null || !results.Any()) return;

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Receipt No", "Supplier", "Delivery Date", "Item Code", "Description", "DN Number", "PO Number", "BC Type", "BC Number", "BC Date", "Qty", "Qty Scan", "Unit", "Currency", "Price", "Amount"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.ReceiptNo);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SupplierName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.DNDate.ToString("dd MMM yyyy"));
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.DNNumber);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PONumber);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BCType);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BCNumber);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BCDate.ToString("dd MMM yyyy"));
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Qty);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.QtyScan);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.UnitClsDescription);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Currency);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Price);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Amount);
        }

        ExcelHelper.AutofitColumns(ws, 1, headers.Count);

        var range = ws.Range(1, 1, rowIdx, headers.Count);
        ExcelHelper.SetBorders(range);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms, false);
        ms.Position = 0;

        _fileStorage.SaveToExports(key, ms);

        int defaultTTLMinute = 5; // simpan file fisik selama 5 menit

        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO ExportFile (FileKey, RegisterDate, TTLMinute)
            VALUES (@key, GETDATE(), @ttl)", new { key, ttl = defaultTTLMinute }, commandType: CommandType.Text);

        await _notificationService.BroadCastOnlyTo([userId], "FileExportExcel", new { KeyFile = key, KeyStorage = key, FileName = "Receipt Inquiry" });
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task ExportPdfReprintBarcode(List<SelectedPrintDto> selectedPrint, string userId, string key)
    {
        var barcodeNos = selectedPrint
            .Select(x => x.Key)
            .ToList();

        var results = await _reprintRepository
            .GetListBarcodeDetail(barcodeNos);

        if (results == null || !results.Any()) return;

        var models = results.Select(item => new LabelBarcodeDetailDto
        {
            BarcodeNo = item.BarcodeNo,
            FromCompany = item.FromCompany,
            ToCompany = item.ToCompany,
            PONumber = item.PONumber,
            ShippingLot = item.ShippingLot,
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            Qty = item.Qty,
            DeliveryDate = item.DeliveryDate,
            DNNumber = item.DNNumber,
            ShippingLabelNo = item.ShippingLabelNo,
        }).ToList();

        var fullHtml = await _renderer.RenderAsync("Templates/PrintBarcodesA4.cshtml", models);
        var pdfBytes = await _renderer.GeneratePdfAsync(fullHtml);

        _fileStorage.SaveToExports(key, new MemoryStream(pdfBytes));

        string keyStorage = Guid.NewGuid().ToString();
        int defaultTTLMinute = 5; // simpan file fisik selama 5 menit

        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO ExportFile (FileKey, RegisterDate, TTLMinute)
            VALUES (@key, GETDATE(), @ttl)", new { key, ttl = defaultTTLMinute }, commandType: CommandType.Text);

        await _notificationService.BroadCastOnlyTo([userId], "FileExportPDF", new { KeyFile = key, KeyStorage = keyStorage, FileName = $"Reprint_Barcode_{DateTime.Now:yyyyMMddHHmmss}"  });
    }
}