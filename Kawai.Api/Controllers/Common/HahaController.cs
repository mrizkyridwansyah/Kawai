using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kawai.Api.Controllers;

[ApiController]
public class HahaController : ControllerBase
{
    private Auth _auth { get; set; }
    protected Auth Auth => _auth ??= GetService<Auth>();

    private IHttpContextAccessor _httpContextAccessor;
    protected IHttpContextAccessor HttpContextAccessor => _httpContextAccessor ??= GetService<IHttpContextAccessor>();
    private IConfiguration configuration { get; set; }
    protected IConfiguration Configuration => configuration ??= GetService<IConfiguration>();
    private IFileStorage _fileStorage { get; set; }
    protected IFileStorage FileStorage => _fileStorage ??= GetService<IFileStorage>();

    private IMemoryCache _memoryCache { get; set; }
    protected IMemoryCache MemoryCache => _memoryCache ??= GetService<IMemoryCache>();

    protected T GetService<T>() => (T)HttpContext.RequestServices.GetService(typeof(T));

    protected ContentResult DataTableResult(RequestParameter param, IEnumerable<object> data = null)
    {
        data ??= Enumerable.Empty<object>();
        int totalRows = 0;

        var firstItem = data.FirstOrDefault();
        if (firstItem != null)
        {
            // Cek apakah item punya properti TotalRows (misal class Warehouse)
            var propInfo = firstItem.GetType().GetProperty("TotalRows");
            if (propInfo != null)
            {
                var val = propInfo.GetValue(firstItem);
                if (val != null)
                    totalRows = Convert.ToInt32(val);
            }
        }

        return new ContentResult
        {
            StatusCode = 200,
            ContentType = "application/json",
            Content = JsonConvert.SerializeObject(new
            {
                Data = new 
                {
                    param.Page,
                    param.Length,
                    //Total = totalRows,
                    Filtered = totalRows,
                    Items = data,
                }
            })
        };
    }

    protected ContentResult Success(object data = null, string message = null, string code = null)
    {
        return new ContentResult
        {
            StatusCode = 200,
            ContentType = "application/json",
            Content = JsonConvert.SerializeObject(new
            {
                Code = code,
                Status = "Success",
                Message = message,
                Data = data,
            })
        };
    }

    protected BadRequestObjectResult Invalid(string message = null)
    {
        return base.BadRequest(new
        {
            Status = "Invalid",
            Message = message,
        });
    }

    protected void AddError(Dictionary<string, List<string>> errors, string key, string message)
    {
        errors ??= [];

        if (!errors.ContainsKey(key))
            errors[key] = [];

        errors[key].Add(message);
    }

    protected BadRequestObjectResult Invalid(object errors = null)
    {
        return BadRequest(new
        {
            Code = "INVALID_REQUEST_DATA",
            Status = "Invalid",
            Message = "INVALID_REQUEST_DATA",
            Errors = errors,
        });
    }

    protected BadRequestObjectResult Invalid(string message = null, object errors = null)
    {
        return BadRequest(new
        {
            Code = "INVALID_REQUEST_DATA",
            Status = "Invalid",
            Message = message,
            Errors = errors,
        });
    }

    protected BadRequestObjectResult Invalid(string message, string code, object data)
    {
        //if (CommandHandler.Messages["EN"].TryGetValue(code, out string value))
        //{
        //    return base.BadRequest(new
        //    {
        //        Code = code,
        //        Data = data,
        //        Status = "Invalid",
        //        Message = value,
        //    });
        //}
        return base.BadRequest(new
        {
            Code = code,
            Data = data,
            Status = "Invalid",
            Message = message,
        });
    }

    protected ContentResult DataNotFound(object value = null)
    {
        return new ContentResult
        {
            StatusCode = 400,
            ContentType = "application/json",
            Content = JsonConvert.SerializeObject(new
            {
                Status = "NotFound",
                Data = value
            })
        };
    }

    protected OkObjectResult EmptyArray(string message = null, string code = null)
    {
        return base.Ok(new
        {
            Code = code,
            Status = "Success",
            Message = message,
            Data = new List<string>(),
        });
    }
}
