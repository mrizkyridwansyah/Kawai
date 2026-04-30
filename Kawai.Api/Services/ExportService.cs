using ClosedXML.Excel;
using Kawai.Api.Hub;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Data;
using System.Text;

namespace Kawai.Api.Services;

/* 
 * Tambahin dulu di interface nya mau export apa aja yg di handle pake background job ini
 */
public interface IExportService
{
    Task ExportExcelItem(RequestParameter param, string userId, string key);
    Task ExportPdfReceiptBarcode(ReceiptDto receipt, string userId);
    Task ExportPdfIQCReportNG(List<QualityCheckReportDto> list, string userId, string key);
}

public class ExportService : IExportService
{
    private readonly RazorViewRenderer _renderer;
    private readonly IFileStorage _fileStorage;
    private readonly IItemRepository _itemRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly NotificationService<NotifApprovalHub> _notificationService;
    private readonly DbExecutor _dbExecutor;

    public ExportService
    (
        IFileStorage fileStorage,
        IItemRepository itemRepository,
        IReceiptRepository receiptRepository,
        NotificationService<NotifApprovalHub> notificationService,
        RazorViewRenderer renderer,
        DbExecutor dbExecutor
    )
    {
        _fileStorage = fileStorage;
        _itemRepository = itemRepository;
        _receiptRepository = receiptRepository;
        _notificationService = notificationService;
        _renderer = renderer;
        _dbExecutor = dbExecutor;
    }

    /* 
     * Ini contoh penerapannya.
     * File akan digenerate lalu disimpan ke directory
     * Setelah itu SignalR akan broadcast ke user yg tadi export kalo file nya udah siap dengan mengirikan Key (ID dari file di direktori)+ FileName (buat jadi nama setelah didownload)
     */
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

    public async Task ExportPdfReceiptBarcode(ReceiptDto receipt, string userId)
    {
        var results = await _receiptRepository.GetListBarcodeDetail(receipt.Id.Value);
        var renderedLabels = new List<string>();

        foreach (var item in results)
        {
            var model = new LabelBarcodeDetailDto
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

            };

            var html = await _renderer.RenderAsync(
                "Templates/PrintBarcode.cshtml",
                model);

            renderedLabels.Add(html);
        }

        var sb = new StringBuilder();

        sb.Append("""
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="utf-8" />
                        <style>
                        @page {
                          size: A4;
                          margin: 10mm;
                        }

                        body {
                          margin: 0;
                          font-family: Arial, sans-serif;
                        }

                        .page {
                          width: 190mm;
                          height: 277mm;
                          display: grid;
                          grid-template-columns: repeat(2, 1fr);
                          grid-template-rows: repeat(4, 1fr);
                          gap: 5mm;
                                    /* pastikan TIDAK ada kotak */
                          border: none;
                          outline: none;
                          box-shadow: none;
                          page-break-after: always;
                        }

                        
                

              .label {
                width: 321px;
                margin: 5px auto;
                background: #fff;
                position: relative;
              }

              table {
                width: 100%;
                border-collapse: collapse;
              }

              td {
                padding: 0;
                vertical-align: top;
              }

              /* ===== HEADER ===== */
              .header {
                background: #0a4f5e;
                color: #fff;
                font-weight: bold;
                height: 25px;
              }

              .header-title {
                font-size: 10px;
                padding-left: 5px;
            	padding-top: 8px;

              }

              .header-page {
                width: 90px;
                text-align: center;
                font-size: 10px;
            	padding-top: 8px;
              }

              /* ===== SHIPPING LOT (OVERLAY) ===== */
              .shipping-lot {
                position: absolute;
                top: 0;
                right: 0;
                width: 50px;
                border-left: 1px solid #000;
                border-bottom: 1px solid #000;
            	 border-right: 1px solid #000;
                background: #fff;
              }

              .shipping-lot-header {
                background: #0a4f5e;
                color: #fff;
                text-align: center;
                padding: 3px 0;
                font-size: 6px;
              }

              .shipping-lot-number {
                text-align: center;
                font-size: 15px;
                font-weight: bold;
                padding: 4px 0;
                margin: 2px;
              }

              /* ===== FROM / TO ===== */
              .fromto td {
                width: 50%;
                padding: 2px 3px;
                border-top: 1px solid #000;
                border-bottom: 1px solid #000;
              }

              .fromto td:first-child {
                border-right: 1px solid #000;
              }

              .small {
              margin-top: 2px;
                font-size: 6px;
                font-weight: bold;
              }

              .big {
                margin-top: 3px;
                font-size: 9px;
                font-weight: bold;
              }

              /* ===== CONTENT ===== */
              .content td {
                padding: 5px;

              }

              .detail td {
                font-size: 10px;
                padding: 3px;


              }

              .lbl {
                width: 70px;

                font-weight: bold;
              }

              .colon {
                width: 5px;
              }

              .qr {
                text-align: right;
              }

              .qr img {
                width: 95px;
                height: 95px;

              }

              /* ===== FOOTER ===== */
              .footer {
                background: #efefef;
                border-top: 1px solid #000;
              }

              .footer td {
                width: 50%;
                padding: 3px 4px;
              }

              .footer td:first-child {
                border-right: 1px solid #000;
              }

              .footer-title {
              margin-top: 2px;
                font-size: 7px;
                font-weight: bold;
              }

              .footer-value {
                margin-top: 8px;
            	 font-size: 10px;
                font-weight: bold;
              }
                        </style>
                    </head>
                    <body>
            """);

        foreach (var chunk in renderedLabels.Chunk(8))
        {
            sb.Append("<div class='page'>");

            foreach (var label in chunk)
            {
                sb.Append("<div class='label'>");
                sb.Append(label);
                sb.Append("</div>");
            }

            sb.Append("</div>");
        }

        sb.Append("""
                    </body>
                    </html>
        """);

        var fullHtml = sb.ToString();
        var pdfBytes = await _renderer.GeneratePdfAsync(fullHtml);

        string key = "PrintBarcodeUsingJob_" + receipt.Id.Value.ToString();
        _fileStorage.SaveToExports(key, new MemoryStream(pdfBytes));

        string keyStorage = Guid.NewGuid().ToString();

        int defaultTTLMinute = 43200;// simpen file fisik nya selama 1 bulan. 

        // Masukkan ke Table ExportFile kalo file hasil export nya mau di hapus
        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO ExportFile (FileKey, RegisterDate, TTLMinute)
            VALUES (@key, GETDATE(), @ttl)", new { key, ttl = defaultTTLMinute }, commandType: CommandType.Text);

        await _notificationService.BroadCastOnlyTo([userId], "FileExportPDF", new { KeyFile = key, KeyStorage = keyStorage, FileName = receipt.SupplierName + "_" + receipt.DNNumber });
    }

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

}