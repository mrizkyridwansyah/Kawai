using Azure.Core;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class PartMaterialRequestOthersRepository : IPartMaterialRequestOthersRepository
{

    private readonly DbExecutor _dbExecutor;

    public PartMaterialRequestOthersRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    
    public async Task<List<PartMaterialRequestOthersDetailDto>> GetListRequestDetail(RequestParameter param)
    {
        var paramRequestId = param.GetParam("RequestId");
        var paramLineCode = param.GetParam("LineCode");
       
        string sp = "sp_Wms_PartMaterialRequestOthers_ListRequestDetail";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestOthersDetailDto>(sp, new
        {
            RequestId = paramRequestId,
            LineCode = paramLineCode, 
        })).ToList();
    }

    public async Task<List<PartMaterialRequestOthersHistoryDto>> GetListHistory(RequestParameter param)
    {
        string sp = "sp_Wms_PartMaterialRequestOthers_ListHistory";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestOthersHistoryDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<PartMaterialRequestOthersListScanDto>> GetListScan(RequestParameter param)
    {
        string sp = "sp_Wms_PartMaterialRequestOthers_ListScan";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestOthersListScanDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<PartMaterialRequestOthersDto> GetDataHeader(long requestid)
    {
        string sp = "sp_Wms_PartMaterialRequestOthers_DataHeader";
        return await _dbExecutor.QueryFirstOrDefaultAsync<PartMaterialRequestOthersDto>(sp, new { RequestId = requestid });
    }
    

      public async Task<List<PartMaterialRequestOthersDto>> DDLSearch(string keyword, string status,   string userId)
    {
        string sp = "sp_Wms_PartMaterialRequestOthers_DDL";

        return (await _dbExecutor.QueryListAsync<PartMaterialRequestOthersDto>(sp, new
        {
            Keyword = keyword ?? "",
            Status = status ?? "",
            UserId = userId
        })).ToList();
    }

   





    public async Task Create(PartMaterialRequestOthers requestothers, string userId)
    {
        requestothers.RequestNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_PartMaterialRequestOthers_GenerateCode");
        var DetaiTest = DataTableHelper.ToDataTable(requestothers.Details);
        string sqlHeader = "sp_Wms_PartMaterialRequestOthers_Create";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            requestothers.RequestNo,
            requestothers.LineCode,
            requestothers.RequestDate,
            Details = DataTableHelper.ToDataTable(requestothers.Details),
            RegisterBy = userId
        });
        requestothers.RequestId = newId;

    }

    public async Task Update(PartMaterialRequestOthers requestothers, string userId)
    {
        string sqlHeader = "sp_Wms_PartMaterialRequestOthers_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            requestothers.RequestId,
            requestothers.RequestNo,
            requestothers.LineCode,
            requestothers.RequestDate,
            Details = DataTableHelper.ToDataTable(requestothers.Details),
            UpdateBy = userId
        });
    }

    
    public async Task Remove(long requestid)
    {
        string sqlHeader = "sp_Wms_PartMaterialRequestOthers_Delete";
        await _dbExecutor.ExecuteAsync(sqlHeader, new { RequestId = requestid });
    }

    
    
    public async Task<Dictionary<string, object>> Capture(long requestid)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_PartMaterialRequestOthers_Capture",
            param: new { RequestId = requestid },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();
               
                return (header, details);
            }
        );

        return new Dictionary<string, object>
        {
            { "Part Material Request Others Header", result.header },
            { "Part Material Request Others Detail", result.details }
        };
    }

 }
