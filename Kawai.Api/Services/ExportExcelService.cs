using ClosedXML.Excel;
using Kawai.Api.Hub;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;

namespace Kawai.Api.Services;
public interface IExportExcelService
{
    Task ExportExcelItem(RequestParameter param, string userId, string key);
}

public class ExportExcelService : IExportExcelService
{
    private readonly IFileStorage _fileStorage;
    private readonly IItemRepository _itemRepository;
    private readonly NotificationService<NotifApprovalHub> _notificationService;

    public ExportExcelService(IFileStorage fileStorage, IItemRepository itemRepository, NotificationService<NotifApprovalHub> notificationService)
    {
        _fileStorage = fileStorage;
        _itemRepository = itemRepository;
        _notificationService = notificationService;
    }

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

        using var ms = new MemoryStream();
        workbook.SaveAs(ms, false);
        ms.Position = 0;

        _fileStorage.SaveToExports(key, ms);

        await _notificationService.BroadCastOnlyTo([userId], "FileExport", new { Key = key, FileName = "List_Item" });
    }
}