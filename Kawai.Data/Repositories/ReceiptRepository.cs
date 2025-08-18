using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
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
    public async Task<ReceiptDto> GetDetail(long id)
    {
        string sp = "sp_Wms_Receipt_Detail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ReceiptDto>(sp, new { ReceiptId = id });
    }

    public async Task<List<ReceiptDetailDto>> GetListDetail(long id)
    {
        string sp = "sp_Wms_Receipt_ListDetail";
        return (await _dbExecutor.QueryListAsync<ReceiptDetailDto>(sp, new { ReceiptId = id })).ToList();
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

}
