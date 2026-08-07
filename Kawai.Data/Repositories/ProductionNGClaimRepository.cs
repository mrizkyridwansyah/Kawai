using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ProductionNGClaimRepository : IProductionNGClaimRepository
{

    private readonly DbExecutor _dbExecutor;

    public ProductionNGClaimRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    
    public async Task<List<ProductionNGClaimDetailDto>> GetListNGDetail(RequestParameter param)
    {
        var paramClaimId = param.GetParam("ClaimId");
        var paramLineCode = param.GetParam("LineCode");
        var paramPickingNo = param.GetParam("PickingNo");

        string sp = "sp_Wms_ProductionNGClaimMaterial_ListNGDetail";
        return (await _dbExecutor.QueryListAsync<ProductionNGClaimDetailDto>(sp, new
        {
            ClaimId = paramClaimId,
            LineCode = paramLineCode,
            PickingNo = paramPickingNo,
        })).ToList();
    }

    public async Task<ProductionNGClaimDto> GetDataHeader(long claimid)
    {
        string sp = "sp_Wms_ProductionNGClaimMaterial_DataHeader";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ProductionNGClaimDto>(sp, new { ClaimId = claimid });
    }
    

      public async Task<List<ProductionNGClaimDto>> DDLSearch(string keyword, string status,   string userId)
    {
        string sp = "sp_Wms_ProductionNGClaimMaterial_DDL";

        return (await _dbExecutor.QueryListAsync<ProductionNGClaimDto>(sp, new
        {
            Keyword = keyword ?? "",
            Status = status ?? "",
            UserId = userId
        })).ToList();
    }

    public async Task<List<ProductionNGClaimDto>> PickingDDLSearch(string keyword, string line, string typeDate, bool showOptionAll, string userId)
    {
        string sp = "sp_Wms_ProductionNGClaimMaterial_PickingDDL";
        return (await _dbExecutor.QueryListAsync<ProductionNGClaimDto>(sp, new
        {
            Keyword = keyword ?? "",
            LineCode = String.IsNullOrEmpty(line) ? "ALL" : line,
            TypeDate = typeDate,
            ShowOptionAll = showOptionAll,
            UserId = userId
        })).ToList();
    }





    public async Task Create(ProductionNGClaim prodngclaim, string userId)
    {
        prodngclaim.ClaimNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_ProductionNGClaimMaterial_GenerateCode");
        var DetaiTest = DataTableHelper.ToDataTable(prodngclaim.Details);
        string sqlHeader = "sp_Wms_ProductionNGClaimMaterial_Create";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            prodngclaim.ClaimNo,
            prodngclaim.LineCode,
            prodngclaim.ClaimDate,
            prodngclaim.Priority,
            prodngclaim.Notes,
            Details = DataTableHelper.ToDataTable(prodngclaim.Details),
            RegisterBy = userId
        });
        prodngclaim.ClaimId = newId;

    }

    public async Task Update(ProductionNGClaim prodngclaim, string userId)
    {
        string sqlHeader = "sp_Wms_ProductionNGClaimMaterial_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            prodngclaim.ClaimId,
            prodngclaim.ClaimNo,
            prodngclaim.LineCode,
            prodngclaim.ClaimDate,
            prodngclaim.Priority,
            prodngclaim.Notes,
            Details = DataTableHelper.ToDataTable(prodngclaim.Details),
            UpdateBy = userId
        });
    }

    public async Task Submit(ProductionNGClaim prodngclaim, string userId)
    {
        string sqlHeader = "sp_Wms_ProductionNGClaimMaterial_Approve";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            prodngclaim.ClaimId,
            prodngclaim.ClaimNo,
            prodngclaim.LineCode,
            prodngclaim.ClaimDate,
            prodngclaim.Priority,
            prodngclaim.Notes,
            Details = DataTableHelper.ToDataTable(prodngclaim.Details),
            UpdateBy = userId
        });
    }

    public async Task Remove(long claimid)
    {
        string sqlHeader = "sp_Wms_ProductionNGClaimMaterial_Delete";
        await _dbExecutor.ExecuteAsync(sqlHeader, new { ClaimId = claimid });
    }

    
    
    public async Task<Dictionary<string, object>> Capture(long claimid)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_ProductionNGClaimMaterial_Capture",
            param: new { ClaimId = claimid },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();
               
                return (header, details);
            }
        );

        return new Dictionary<string, object>
        {
            { "Claim Production Header", result.header },
            { "Claim Production Detail", result.details }
        };
    }

 }
