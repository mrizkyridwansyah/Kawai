using Dapper;
using Kawai.Api.Models;
using Kawai.Data;
using Kawai.Data.SqlConnections;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.CodeDom.Compiler;
using System.Data;
namespace Kawai.Api;

public class Auth(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, DbExecutor db)
{
    private readonly DbExecutor _db = db;
    protected IMemoryCache MemoryCache { get; set; } = memoryCache;
    protected IHttpContextAccessor HttpContextAccessor { get; set; } = httpContextAccessor;

    public string Token
    {
        get
        {
            var authorizationHeader = HttpContextAccessor.HttpContext.Request.Headers.Authorization.ToString();

            var cookies = HttpContextAccessor.HttpContext.Request.Cookies["__SIDX"];

            if (string.IsNullOrWhiteSpace(authorizationHeader) && string.IsNullOrWhiteSpace(cookies))
                return default;

            try
            {
                if (!string.IsNullOrWhiteSpace(authorizationHeader))
                    return authorizationHeader["bearer".Length..].Trim();

                if (!string.IsNullOrWhiteSpace(cookies))
                    return cookies;

                return default;
            }
            catch { return default; }
        }
    }

    public User User
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Token)) return default;

            var key = $"SESSION_{Token}";
            var data = MemoryCache.Get<User>(key);

            if (data == null)
            {
                var session = _db.QueryFirstOrDefaultAsync<Session>(@"SELECT TOP 1 * FROM Sessions WHERE Token = @Token", new { Token }, CommandType.Text);
                if (session.Result == null) return default;

                var user = _db.QueryFirstOrDefaultAsync<User>(@"SELECT TOP 1 * FROM SS_UserSetup WHERE UserID = @UserId", new { session.Result.UserId }, CommandType.Text);
                if (user == null) return default;

                data = MemoryCache.Set(key, user.Result, DateTimeOffset.Now.AddHours(1));
            }

            return data;
        }
    }
}
