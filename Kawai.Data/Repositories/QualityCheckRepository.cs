using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class QualityCheckRepository : IQualityCheckRepository
{
    private readonly DbExecutor _dbExecutor;

    public QualityCheckRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<QualityCheckDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_QualityCheck_List";
        return (await _dbExecutor.QueryListAsync<QualityCheckDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<QualityCheckDto> GetData(string id)
    {
        string sp = "sp_Wms_QualityCheck_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<QualityCheckDto>(sp, new { Id = id });
    }

    public async Task Confirm(QualityCheck qualitycheck, string userId)
    {
        string sql = @"sp_Wms_QualityCheck_Confirm";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            qualitycheck.Id,
            qualitycheck.QC_Status,
            qualitycheck.Remarks,
            UpdateBy = userId
        });
    }



    public async Task<Dictionary<string, object>> Capture(string id)
    {
        string sp = "sp_Wms_QualityCheck_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Id = id });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<List<QualityCheckDto>> DDLDNNoBySupplierDateFrom(string keyword, string supplier, string receiptdatefrom, string receiptdateto)
    {
        string sp = "sp_Wms_QualityCheck_DDLDNNoBySupplierDateFrom";
        return (await _dbExecutor.QueryListAsync<QualityCheckDto>(sp, new
        {
            Keyword = keyword ?? "",
            SupplierCode = !String.IsNullOrEmpty(supplier) ? supplier : "ALL",
            ReceiptDateFrom = !String.IsNullOrEmpty(receiptdatefrom) ? receiptdatefrom : "ALL",
            ReceiptDateTo = !String.IsNullOrEmpty(receiptdateto) ? receiptdateto : "ALL" 
        })).ToList();
    }





}
