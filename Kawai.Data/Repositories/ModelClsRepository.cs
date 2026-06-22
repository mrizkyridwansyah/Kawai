using Kawai.Api.DTOs;
using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using System.Data;

namespace Kawai.Data.Repositories;

public class ModelClsRepository : IModelClsRepository
{
    private readonly DbExecutor _dbExecutor;

    public ModelClsRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ModelClsDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_ModelCls_List";
        return (await _dbExecutor.QueryListAsync<ModelClsDto>(sp, param.ToQueryObject())).ToList();
    }

     
    public async Task<ModelClsDto> GetData(string model_cls)
    {
        string sp = "sp_Wms_ModelCls_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ModelClsDto>(sp, new { Model_Cls = model_cls });
    }

   
    public async Task Update(string model_cls, ModelCls modelcls, string updateUser)
    {
        string sql = @"sp_Wms_ModelCls_Update";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            modelcls.Model_Cls,
            Description = modelcls.Description,
            CycleTime = modelcls.CycleTime,
            Picture = modelcls.ImageName,
            UpdateBy = updateUser
        });
    }

    public async Task Remove(string model_cls, string deletedUser)
    {
        string sql = "sp_Wms_ModelCls_Delete";
        await _dbExecutor.ExecuteAsync(sql, new { Model_Cls = model_cls });
    }

    public async Task<Dictionary<string, object>> Capture(string model_cls)
    {
        string sp = "sp_Wms_ModelCls_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Model_Cls = model_cls });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

  
}
