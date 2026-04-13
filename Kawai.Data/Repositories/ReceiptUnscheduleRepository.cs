using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ReceiptUnscheduleRepository : IReceiptUnscheduleRepository
{

    private readonly DbExecutor _dbExecutor;

    public ReceiptUnscheduleRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ItemPackingSupplierDto>> GetListItem(RequestParameter param)
    {
        string sp = "sp_Wms_ReceiptUnschedule_ListItem";
        return (await _dbExecutor.QueryListAsync<ItemPackingSupplierDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task Create(ReceiptUnschedule receipt, string userId)
    {
        receipt.ReceiptNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Receipt_GenerateCode", new { receipt.FactoryCode });

        string sqlHeader = "sp_Wms_ReceiptUnschedule_Create";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            receipt.ReceiptNo,
            receipt.DNNumber,
            receipt.FactoryCode,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            receipt.Transport,
            receipt.ReferenceNo,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            RegisterBy = userId
        });
        receipt.Id = newId;
    }

    public async Task Update(ReceiptUnschedule receipt, string userId)
    {
        string sqlHeader = "sp_Wms_ReceiptUnschedule_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            receipt.Id,
            receipt.DNNumber,
            receipt.FactoryCode,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            receipt.Transport,
            receipt.ReferenceNo,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            UpdateBy = userId
        });
    }
}
