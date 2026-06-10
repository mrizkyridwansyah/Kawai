using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using Kawai.Data.SqlConnections;
using Kawai.Api.Models;
using Kawai.Data;
using Microsoft.AspNetCore.Http;
using System;
using Kawai.Api.DTOs;

namespace Kawai.Api;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow if user has a valid active session
        var token = GetToken(httpContext);
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var memoryCache = httpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var db = httpContext.RequestServices.GetRequiredService<DbExecutor>();

        var cacheKey = $"SESSION_{token}";
        var user = memoryCache.Get<User>(cacheKey);

        if (user != null)
        {
            return user.IsAdmin;
        }

        try
        {
            var session = db.QueryFirstOrDefaultAsync<Session>(
                @"SELECT TOP 1 * FROM Sessions WHERE Token = @Token",
                new { Token = token },
                CommandType.Text
            ).GetAwaiter().GetResult();

            if (session == null) return false;

            var dbUser = db.QueryFirstOrDefaultAsync<User>(
                @"SELECT TOP 1 UserID, FullName, IsAdmin = StatusAdmin FROM SS_UserSetup WHERE UserID = @UserId",
                new { UserId = session.UserId },
                CommandType.Text
            ).GetAwaiter().GetResult();

            if (dbUser != null)
            {
                memoryCache.Set(cacheKey, dbUser, DateTimeOffset.Now.AddHours(1));
                return dbUser.IsAdmin;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private string GetToken(HttpContext httpContext)
    {
        var authorizationHeader = httpContext.Request.Headers.Authorization.ToString();
        var cookies = httpContext.Request.Cookies["__SIDXTrial"];

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            try
            {
                if (authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
                {
                    return authorizationHeader["bearer".Length..].Trim();
                }
            }
            catch { }
        }

        if (!string.IsNullOrWhiteSpace(cookies))
        {
            return cookies;
        }

        return null;
    }
}
