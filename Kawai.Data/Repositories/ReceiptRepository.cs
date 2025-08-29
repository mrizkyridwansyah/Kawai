using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ReceiptRepository : IReceiptRepository
{

    private readonly DbExecutor _dbExecutor;

    public ReceiptRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ReceiptDto>> GetList(RequestParameter param)
    {
        string sp = "sp_Wms_Receipt_List";
        return (await _dbExecutor.QueryListAsync<ReceiptDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<ReceiptDto> GetDataHeader(long id)
    {
        string sp = "sp_Wms_Receipt_DataHeader";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ReceiptDto>(sp, new { ReceiptId = id });
    }
    public async Task<List<ReceiptDetailDto>> GetListDetail(long receiptId)
    {
        string sp = "sp_Wms_Receipt_ListDetail";
        return (await _dbExecutor.QueryListAsync<ReceiptDetailDto>(sp, new { ReceiptId = receiptId })).ToList();
    }

    public async Task<List<ReceiptDetailBarcodeDto>> GetListDetailBarcode(long receiptId)
    {
        string sp = "sp_Wms_Receipt_ListDetailBarcode";
        return (await _dbExecutor.QueryListAsync<ReceiptDetailBarcodeDto>(sp, new { ReceiptId = receiptId })).ToList();
    }

    public async Task<ReceiptDetailBarcodeDto> GetDataBarcode(long id, string barcodeNo)
    {
        string sp = "sp_Wms_Receipt_DataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ReceiptDetailBarcodeDto>(sp, new { ReceiptId = id, BarcodeNo = barcodeNo });
    }

    public async Task<List<ReceiptDto>> DDLSearchReceipt(string keyword, string status)
    {
        string sp = "sp_Wms_Receipt_DDLSearchReceipt";
        return (await _dbExecutor.QueryListAsync<ReceiptDto>(sp, new { Keyword = keyword ?? "", Status = status ?? "" })).ToList();
    }

    public async Task Create(Receipt receipt, string userId)
    {
        receipt.ReceiptNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Receipt_GenerateCode");
        string sqlHeader = "sp_Wms_Receipt_Create";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            receipt.ReceiptNo,
            receipt.IsManual,
            receipt.DNNumber,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            RegisterBy = userId
        });
        receipt.Id = newId;
    }

    public async Task CreateUsingMutation(Receipt receipt, string userId)
    {
        receipt.ReceiptNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Receipt_GenerateCode");
        string sqlHeader = "sp_Wms_Receipt_CreateWithMutation";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            receipt.ReceiptNo,
            receipt.IsManual,
            receipt.DNNumber,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            RegisterBy = userId
        });
        receipt.Id = newId;
    }

    public async Task Update(Receipt receipt, string userId)
    {
        string sqlHeader = "sp_Wms_Receipt_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            receipt.Id,
            receipt.DNNumber,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            UpdateBy = userId
        });
    }

    public async Task Remove(long id)
    {
        string sqlHeader = "sp_Wms_Receipt_Delete";
        await _dbExecutor.ExecuteAsync(sqlHeader, new { Id = id });
    }

    public async Task Verify(MobileReceipt payload, bool updateStock, string userId)
    {
        string sqlHeader = "sp_Wms_Receipt_Verify";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            payload.Id,
            payload.BarcodeNo,
            payload.Qty,
            payload.QtyVerify,
            IsUpdateStock = updateStock,
            VerifiedBy = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(long id)
    {
        string sp = "sp_Wms_Receipt_CaptureHeader";
        var header = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Id = id });

        string spDetail = "sp_Wms_Receipt_CaptureListDetail";
        var detail = (await _dbExecutor.QueryListAsync<dynamic>(spDetail, new { ReceiptId = id })).ToList();

        return new Dictionary<string, object>
        {
            { "Header", header },
            { "Detail", detail }
        };
    }

    public async Task<Dictionary<string, object>> CaptureDataBarcode(long id)
    {
        string sp = "sp_Wms_Receipt_CaptureDataBarcode";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Id = id });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }
}
