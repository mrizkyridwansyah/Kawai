using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Kawai.Api.Shared.Extension;
using Kawai.Data.SqlConnections;
using System.Data;
using Kawai.Api;
using System.Text;
using Serilog;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var serviceProvider = context.HttpContext.RequestServices;
        var auth = serviceProvider.GetRequiredService<Auth>();
        var logExecutor = serviceProvider.GetRequiredService<LogExecutor>();
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
            var sql = @"
                    INSERT INTO ErrorLogs
                    (Date, Message, Method, UserAgent, RemoteAddr, RequestPath, RequestBody, StackTrace, UserId, FullName, StatusCode)
                    VALUES
                    (@Date, @Message, @Method, @UserAgent, @RemoteAddr, @RequestPath, @RequestBody, @StackTrace, @UserId, @FullName, @StatusCode);
                ";

            var log = new
            {
                Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Message = "Request data is not valid.",
                request.Method,
                UserAgent = request.Headers["User-Agent"].ToString(),
                RemoteAddr = connection.RemoteIpAddress?.ToString(),
                RequestPath = request.Path.ToString(),
                RequestBody = requestBody,
                StackTrace = "", // Stack trace optional kalau ada exception handling
                StatusCode = 400,
                UserId = auth?.User?.UserID ?? "-",
                FullName = auth?.User?.FullName ?? "-"
            };

            logExecutor.Execute(sql, log, commandType: CommandType.Text);

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
