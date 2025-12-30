using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class NGClaimRepository : INGClaimRepository
{

    private readonly DbExecutor _dbExecutor;

    public NGClaimRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<NGClaimDto>> GetList(RequestParameter param)
    {
        string sp = "sp_Wms_NGClaimMaterial_List";
        return (await _dbExecutor.QueryListAsync<NGClaimDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<List<NGClaimDetailDto>> GetListPODetail(RequestParameter param)
    {
        var paramClaimId = param.GetParam("ClaimId");
        var paramSupplier = param.GetParam("SupplierCode");
        var paramDateFrom = param.GetParam("DateFrom");
        var paramDateUntil = param.GetParam("DateUntil");

        string sp = "sp_Wms_NGClaimMaterial_ListPODetail";
        return (await _dbExecutor.QueryListAsync<NGClaimDetailDto>(sp, new
        {
            ClaimId = paramClaimId,
            SupplierCode = paramSupplier,
            DateFrom = paramDateFrom,
            DateUntil = paramDateUntil,
        })).ToList();
    }

    public async Task<List<NGClaimDetailDto>> GetListNGClaimDetail(RequestParameter param)
    {
        var paramClaimId = param.GetParam("ClaimId");
        var paramSupplier = param.GetParam("SupplierCode");
        var paramDateFrom = param.GetParam("DateFrom");
        var paramDateUntil = param.GetParam("DateUntil");

        string sp = "sp_Wms_NGClaimMaterial_ListNGClaimDetail";
        return (await _dbExecutor.QueryListAsync<NGClaimDetailDto>(sp, new
        {
            ClaimId = paramClaimId,
            SupplierCode = paramSupplier,
            DateFrom = paramDateFrom,
            DateUntil = paramDateUntil,
        })).ToList();
    }

    public async Task<NGClaimDto> GetDataHeader(long claimid)
    {
        string sp = "sp_Wms_NGClaimMaterial_DataHeader";
        return await _dbExecutor.QueryFirstOrDefaultAsync<NGClaimDto>(sp, new { ClaimId = claimid });
    }
    //public async Task<List<NGClaimDetailDto>> GetListDetail(long claimid)
    //{
    //    string sp = "sp_Wms_NGClaimMaterial_ListDetail";
    //    return (await _dbExecutor.QueryListAsync<NGClaimDetailDto>(sp, new { ClaimId = claimid })).ToList();
    //}

      public async Task<List<NGClaimDto>> DDLSearch(string keyword,   string supplier, DateTime? periodFrom, DateTime? periodUntil, string status,   string userId)
    {
        string sp = "sp_Wms_NGClaimMaterial_DDL";

        return (await _dbExecutor.QueryListAsync<NGClaimDto>(sp, new
        {
            Keyword = keyword ?? "",
            Status = status ?? "",
            SupplierCode = supplier?? "",
            PeriodFrom = periodFrom,
            PeriodUntil = periodUntil,
            UserId = userId
        })).ToList();
    }

    

    public async Task Create(NGClaim ngclaim, string userId)
    {
        ngclaim.ClaimNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_NGClaimMaterial_GenerateCode");
        var DetaiTest = DataTableHelper.ToDataTable(ngclaim.Details);
        string sqlHeader = "sp_Wms_NGClaimMaterial_Create";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            ngclaim.ClaimNo,
            ngclaim.DNNumber,
            ngclaim.SupplierCode,
            ngclaim.DNDate,
            ngclaim.BCNumber,
            ngclaim.BCType,
            ngclaim.BCDate,
            ngclaim.VehicleNo,
            ngclaim.Transport,
            Details = DataTableHelper.ToDataTable(ngclaim.Details),
            RegisterBy = userId
        });
        ngclaim.ClaimId = newId;
    }

    public async Task Update(NGClaim ngclaim, string userId)
    {
        string sqlHeader = "sp_Wms_NGClaimMaterial_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            ngclaim.ClaimId,
            ngclaim.DNNumber,
            ngclaim.SupplierCode,
            ngclaim.DNDate,
            ngclaim.BCNumber,
            ngclaim.BCType,
            ngclaim.BCDate,
            ngclaim.VehicleNo,
            ngclaim.Transport,
            Details = DataTableHelper.ToDataTable(ngclaim.Details),
            UpdateBy = userId
        });
    }

    public async Task Approve(NGClaim ngclaim, string userId)
    {
        string sqlHeader = "sp_Wms_NGClaimMaterial_Approve";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            ngclaim.ClaimId,
            ngclaim.DNNumber,
            ngclaim.SupplierCode,
            ngclaim.DNDate,
            ngclaim.BCNumber,
            ngclaim.BCType,
            ngclaim.BCDate,
            ngclaim.VehicleNo,
            ngclaim.Transport,
            Details = DataTableHelper.ToDataTable(ngclaim.Details),
            UpdateBy = userId
        });
    }

    public async Task Remove(long claimid)
    {
        string sqlHeader = "sp_Wms_NGClaimMaterial_Delete";
        await _dbExecutor.ExecuteAsync(sqlHeader, new { ClaimId = claimid });
    }

    public async Task PrintLabel(long claimid, string userId)
    {
        string sqlHeader = "sp_Wms_NGClaimMaterial_PrintLabel";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            ClaimId = claimid,
            UserId = userId
        });
    }

    
    public async Task<Dictionary<string, object>> Capture(long claimid)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_NGClaimMaterial_Capture",
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
            { "Claim Header", result.header },
            { "Claim Detail", result.details }
        };
    }

 }
