using Kawai.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kawai.Data;
using Kawai.Data.SqlConnections;

namespace Kawai.Api;

public interface ISessionManager
{
    Task<SessionResult> Authenticate(string Username, string Password);
    Task<SessionResult> GenerateSession(string Username);
    Task<SessionResult> ValidateToken(string token);
    //Task<PrivilegeResult> ValidatePrivilege(string featureId);
    void RemoveSession(string token);
}

public class SessionResult
{
    public string Token { get; set; }
    public bool IsSucceeded { get; set; }
    public Session Session { get; set; }
}

public class SessionManager(Auth auth,
    IHttpContextAccessor httpContextAccessor,
    IMemoryCache cacheManager,
    DbExecutor dbExecutor) : ISessionManager
{
    private readonly string CacheKey = "__session_manager";
    private readonly DbExecutor _dbExecutor = dbExecutor;

    public List<Session> Sessions
    {
        get
        {
            if (MemoryCache.Get(CacheKey) is List<Session> existing)
                return existing;
            return [];
        }
        set
        {
            MemoryCache.Set(CacheKey, value);
        }
    }

    //public List<PrivilegeResult> PrivilegeResults
    //{
    //    get
    //    {
    //        if (MemoryCache.Get($"{CacheKey}_privilege_result") is List<PrivilegeResult> result)
    //            return result;
    //        return [];
    //    }
    //}

    protected IMemoryCache MemoryCache { get; set; } = cacheManager;
    protected string RemoteAddr { get; set; } = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "0.0.0.0";
    protected string UserAgent { get; set; } = httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "-";
    protected HttpRequest Request { get; } = httpContextAccessor.HttpContext?.Request;

    protected Auth Auth { get; } = auth;

    public async Task<SessionResult> Authenticate(string Username, string Password)
    {
        if (string.IsNullOrWhiteSpace(Username))
            return new SessionResult { IsSucceeded = false };

        try
        {
            var user = await _dbExecutor.QueryFirstOrDefaultAsync<User>(@"sp_Wms_User_CheckLogin", new { UserID = Username, Password = Password.Encrypt(Username.ToUpper()) });

            if (user == null)
                return new SessionResult { IsSucceeded = false };

            var session = new Session
            {
                UserId = user.UserID,
                Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ExpiryDate = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds(),// LIFETIME NYA INI
                UserAgent = UserAgent,
                RemoteAddr = RemoteAddr,
                Token = Guid.NewGuid().UniqueId(100),
            };

            await _dbExecutor.ExecuteAsync(@"
            INSERT INTO Sessions (UserId, Date, ExpiryDate, UserAgent, RemoteAddr, Token)
            VALUES (@UserId, @Date, @ExpiryDate, @UserAgent, @RemoteAddr, @Token)", session, commandType: CommandType.Text);

            Sessions.Add(session);
            MemoryCache.Set(CacheKey, Sessions);

            return new SessionResult
            {
                IsSucceeded = true,
                Token = session.Token,
                Session = session
            };
        }
        catch (Exception ex)
        {
            throw;
        }


    }

    public async Task<SessionResult> GenerateSession(string Username)
    {
        var user = await _dbExecutor.QueryFirstOrDefaultAsync<User>(
            @"SELECT TOP 1 * FROM SS_UserSetup WHERE UserID = @Username",
            new { Username }, commandType: CommandType.Text);

        if (user == null)
            return new SessionResult { IsSucceeded = false };

        var session = new Session
        {
            UserId = user.UserID,
            Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ExpiryDate = DateTimeOffset.UtcNow.AddHours(12).ToUnixTimeMilliseconds(),
            UserAgent = UserAgent,
            RemoteAddr = RemoteAddr,
            Token = Guid.NewGuid().UniqueId(100),
        };

        await _dbExecutor.ExecuteAsync(@"
            INSERT INTO Sessions (UserId, Date, ExpiryDate, UserAgent, RemoteAddr, Token)
            VALUES (@UserId, @Date, @ExpiryDate, @UserAgent, @RemoteAddr, @Token)", session, commandType: CommandType.Text);

        Sessions.Add(session);
        MemoryCache.Set(CacheKey, Sessions);

        return new SessionResult
        {
            IsSucceeded = true,
            Token = session.Token,
            Session = session
        };
    }

    public async Task<SessionResult> ValidateToken(string token)
    {
        var dateNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var session = await _dbExecutor.QueryFirstOrDefaultAsync<Session>(
            @"SELECT TOP 1 * FROM Sessions WHERE Token = @Token AND (ExpiryDate IS NULL OR ExpiryDate >= @Now)",
            new { Token = token, Now = dateNow }, commandType: CommandType.Text);

        if (session == null)
            return new SessionResult { IsSucceeded = false };

        Sessions.Add(session);
        MemoryCache.Set(CacheKey, Sessions);

        return new SessionResult
        {
            IsSucceeded = true,
            Token = session.Token,
            Session = session
        };
    }

    //public async Task<PrivilegeResult> ValidatePrivilege(string featureId)
    //{
    //    var defaultResult = new PrivilegeResult
    //    {
    //        UserId = Auth?.User?.UserID ?? "",
    //        FeatureId = featureId,
    //        Granted = false
    //    };

    //    //if (Auth?.Role?.IsAdministrator == true)
    //    //{
    //    //    defaultResult.Granted = true;
    //    //    return defaultResult;
    //    //}

    //    if (string.IsNullOrWhiteSpace(featureId))
    //        return defaultResult;

    //    var privilegeResult = PrivilegeResults
    //        .FirstOrDefault(p => p.UserId == Auth.User.UserID && p.FeatureId == featureId);

    //    if (privilegeResult != null)
    //        return privilegeResult;

    //    //var exists = await _db.ExecuteScalarAsync<int>(
    //    //    @"SELECT COUNT(1) 
    //    //      FROM UserRolePrivileges 
    //    //      WHERE RoleId = @RoleId AND FeatureId = @FeatureId",
    //    //    //new { RoleId = Auth.User.RoleId, FeatureId = featureId });
    //    //    new { FeatureId = featureId });

    //    //if (exists > 0)
    //    //{
    //    //    defaultResult.Granted = true;
    //    //    var list = PrivilegeResults;
    //    //    list.Add(defaultResult);
    //    //    MemoryCache.Set($"{CacheKey}_privilege_result", list);
    //    //}

    //    return defaultResult;
    //}

    public void RemoveSession(string token)
    {
        var session = Sessions.FirstOrDefault(p => p.Token == token);
        if (session != null)
        {
            Sessions.Remove(session);
            MemoryCache.Set(CacheKey, Sessions);
        }
    }
}

public class PrivilegeResult
{
    public string UserId { get; set; }
    public string FeatureId { get; set; }
    public bool Granted { get; set; }
}
