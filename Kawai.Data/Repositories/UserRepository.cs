using Kawai.Api.DTOs;
using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbExecutor _dbExecutor;

    public UserRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<UserDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_User_List";
        return (await _dbExecutor.QueryListAsync<UserDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<UserDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_User_DDL";
        return (await _dbExecutor.QueryListAsync<UserDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<JobPositionDto>> GetJobPositionDDL(string keyword)
    {
        string sp = "sp_Wms_JobPostion_DDL";
        return (await _dbExecutor.QueryListAsync<JobPositionDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<UserDto> GetData(string userId)
    {
        string sp = "sp_Wms_User_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<UserDto>(sp, new { UserID = userId });
    }

    public async Task Create(User user, string registerUser)
    {
        string sql = @"sp_Wms_User_Create";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            user.UserID,
            user.FullName,
            user.Password,
            AdminStatus = user.IsAdmin,
            JobPosition = user.JobPositionCode,
            UserGroup = user.UserGroupID,
            UserPhoto = user.ImageName,
            RegisterBy = registerUser
        });
    }

    public async Task Update(string userId, User user, string updateUser)
    {
        string sql = @"sp_Wms_User_Update";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            user.UserID,
            user.FullName,
            user.Password,
            AdminStatus = user.IsAdmin,
            JobPosition = user.JobPositionCode,
            UserGroup = user.UserGroupID,
            UserPhoto = user.ImageName,
            UpdateBy = updateUser
        });
    }

    public async Task Remove(string userId, string deletedUser)
    {
        string sql = "sp_Wms_User_Delete";
        await _dbExecutor.ExecuteAsync(sql, new { UserID = userId });
    }

    public async Task<Dictionary<string, object>> Capture(string userId)
    {
        string sp = "sp_Wms_User_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { UserID = userId });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task ChangePassword(string userId, string newPassword)
    {
        string sql = @"sp_Wms_User_ChangePassword";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            UserID = userId,
            Password = newPassword,
        });
    }
}
