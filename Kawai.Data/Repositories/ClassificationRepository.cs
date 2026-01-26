using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ClassificationRepository : IClassificationRepository
{
    private readonly DbExecutor _dbExecutor;

    public ClassificationRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ClassificationDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Classification_ListTab";
        return (await _dbExecutor.QueryListAsync<ClassificationDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<List<ClassificationTableDto>> GetListTableDetail(string tablename)
    {
        string sp = "sp_Wms_Classification_TableListDetail";
        return (await _dbExecutor.QueryListAsync<ClassificationTableDto>(sp, new { TableName = tablename })).ToList();
    }

    public async Task<ClassificationTableDto> GetData(string code, string tablename)
    {
        string sp = "sp_Wms_Classification_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ClassificationTableDto>(sp, new { Code = code, TableName = tablename });
    }

 


    public async Task Create(Classification classification, string userId)
    {
        string sql = @"sp_Wms_Classification_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            classification.Description,
            classification.Code,
            classification.TableName,
            RegisterBy = userId
        });
    }

    public async Task Update(Classification classification, string userId)
    {
        string sql = @"sp_Wms_Classification_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            classification.Description,
            classification.Code,
            classification.TableName,
             
            UpdateBy = userId
        });
    }

    public async Task Remove(string code, string tablename, string userId)
    {
        string sql = "sp_Wms_Classification_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { Code = code, TableName = tablename });
    }

    public async Task<Dictionary<string, object>> Capture(string code , string tablename)
    {
        string sp = "sp_Wms_Classification_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Code = code , TableName = tablename });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }



}
