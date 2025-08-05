using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Models.Log;
using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace Kawai.Api;

public class DataLogger(Auth auth, IHttpContextAccessor contextAccessor, LogExecutor logExecutor)
{
    protected HttpContext Context => contextAccessor.HttpContext;
    private readonly LogExecutor _logExecutor = logExecutor;

    public async Task SaveDataLog(DataLogDto data)
    {
        //var json = JsonConvert.SerializeObject(Compare(data.Before, data.After));
        var json = JsonConvert.SerializeObject(new Dictionary<string, object>
        {
            { "Before", data.Before ?? new Dictionary<string, object>() },
            { "After", data.After ?? new Dictionary<string, object>() }
        });


        var log = new DataLog
        {
            Action = data.Action.ToString(),
            Activity = string.IsNullOrEmpty(data.Activity) ? data.Action.ToString() : data.Activity,
            DocumentType = data.DocumentType,
            ReferenceId = data.ReferenceId,
            EntityId = data.EntityId,
            Date = EpochDateTime.Now,
            Data = json,
            Method = Context.Request.Method,
            RequestPath = Context.Request.Path,
            RemoteAddr = Context.Connection.RemoteIpAddress.MapToIPv4().ToString(),
            UserAgent = Context.Request.Headers.UserAgent.ToString(),
            UserId = auth.User.UserID,
            FullName = auth.User.FullName,
        };

        await _logExecutor.ExecuteAsync(@"
            INSERT INTO [dbo].[DataLogs]
            ([Date], [UserId], [FullName], [UserAgent], [RemoteAddr], [Method], [RequestPath], [Action], [Activity], [DocumentType], [EntityId], [ReferenceId], [Data], [ElapsedMilliseconds])
            VALUES
            (@Date, @UserId, @FullName, @UserAgent, @RemoteAddr, @Method, @RequestPath, @Action, @Activity, @DocumentType, @EntityId, @ReferenceId, @Data, 0)", log, commandType: CommandType.Text);
    }

    protected static IDictionary<string, IDictionary<string, object>> Compare(
       Dictionary<string, object> before = null,
       Dictionary<string, object> after = null)
    {
        var bef = new Dictionary<string, object>();
        var aft = new Dictionary<string, object>();

        var allKeys = new HashSet<string>(before?.Keys ?? Enumerable.Empty<string>())
            .Union(after?.Keys ?? Enumerable.Empty<string>());

        foreach (var key in allKeys)
        {
            object v1 = null;
            object v2 = null;

            before?.TryGetValue(key, out v1);
            after?.TryGetValue(key, out v2);

            var val1 = FormatValue(v1);
            var val2 = FormatValue(v2);

            if (!Equals(val1, val2))
            {
                bef[key] = val1;
                aft[key] = val2;
            }
        }

        return new Dictionary<string, IDictionary<string, object>>
       {
           { "Before", bef },
           { "After", aft },
       };
    }

    private static object FormatValue(object value)
    {
        if (value == null)
            return "";

        return value switch
        {
            bool b => b ? "YA" : "TIDAK",
            double d => d.ToString("N0"),
            decimal dec => dec.ToString("N0"),
            float f => f.ToString("N0"),
            long l => l.ToString("N0"),
            int i => i.ToString("N0"),
            DateTime dt => dt.ToString("g"),
            _ => value.ToString()
        };
    }

}

public static class PropertyInfoExtensions
{
    public static string GetDefaultName(this PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<DisplayAttribute>();
        return attr == null ? property.Name : attr.Name;
    }
}


