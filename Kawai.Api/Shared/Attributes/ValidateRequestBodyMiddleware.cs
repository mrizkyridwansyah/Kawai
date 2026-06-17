using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Kawai.Api.Shared.Extension;
using Kawai.Api.Services.Logging;
using Kawai.Api;
using System.Text;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var serviceProvider = context.HttpContext.RequestServices;
        var auth = serviceProvider.GetRequiredService<Auth>();
        var allErrors = new Dictionary<string, List<string>>();

        foreach (var arg in context.ActionArguments)
        {
            var model = arg.Value;
            if (model == null) continue;

            var result = model.TryValidateRecursive(serviceProvider);
            if (result.Count == 0) continue;

            foreach (var error in result)
            {
                if (!allErrors.ContainsKey(error.Key))
                    allErrors[error.Key] = [];

                allErrors[error.Key].AddRange(error.Value);
            }

            var http = context.HttpContext;
            var request = http.Request;
            var connection = http.Connection;
            var user = http.User;

            string requestBody = "";
            if (request.ContentLength > 0 && request.Body.CanSeek)
            {
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = reader.ReadToEnd();
                request.Body.Position = 0; // reset supaya bisa dibaca controller
            }
            var logBuffer = serviceProvider.GetRequiredService<LogBufferService>();

            logBuffer.EnqueueErrorLog(new ErrorLogEntry
            {
                Date = new EpochDateTime(DateTime.UtcNow.ToUnixTimeMilliseconds()).Value,
                Message = "Request data is not valid.",
                Method = request.Method,
                UserAgent = request.Headers["User-Agent"].ToString(),
                RemoteAddr = connection.RemoteIpAddress?.ToString(),
                RequestPath = request.Path.ToString(),
                RequestBody = requestBody,
                StackTrace = "",
                StatusCode = 400,
                UserId = auth?.User?.UserID ?? "-",
                FullName = auth?.User?.FullName ?? "-"
            });

            context.Result = new BadRequestObjectResult(new
            {
                Code = "INVALID_REQUEST_DATA",
                Status = "Invalid",
                Message = "Request data is not valid.",
                Errors = allErrors
            });

            return;
        }
    }
}
