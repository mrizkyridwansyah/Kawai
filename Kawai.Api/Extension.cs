using Kawai.Data;
using Kawai.Data.SqlConnections;
using Kawai.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace Kawai.Api;

public static class Extension
{
    public static void UseMorphErrorHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(a => a.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            if (exception == null)
            {
                exception = new Exception("An unidentified error occurred.");
            }

            var typeName = exception.GetType().Name;
            string message = exception.Message;

            int statusCode = exception switch
            {
                HttpCustomException customEx => customEx.StatusCode,
                SqlException => 400,
                _ => 500
            };

            string requestBody = null;
            try
            {
                if (context.Request.Body.CanSeek)
                {
                    context.Request.Body.Position = 0;
                }

                if (context.Request.ContentType?.StartsWith("multipart/form-data") == true)
                {
                    var parsedForm = await ParseMultipartFormAsync(context.Request);
                    requestBody = JsonConvert.SerializeObject(parsedForm);
                }
                else
                {
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                }

                if (context.Request.Body.CanSeek)
                {
                    context.Request.Body.Position = 0;
                }
            }
            catch
            {
                requestBody = "[Failed to read request body]";
            }

            try
            {
                var auth = context.RequestServices.GetService<Auth>();
                var logExecutor = context.RequestServices.GetService<LogExecutor>();

                if (logExecutor != null)
                {
                    var sql = @"
                        INSERT INTO ErrorLogs
                        (Date, Message, Method, UserAgent, RemoteAddr, RequestPath, RequestBody, StackTrace, UserId, FullName, StatusCode)
                        VALUES
                        (@Date, @Message, @Method, @UserAgent, @RemoteAddr, @RequestPath, @RequestBody, @StackTrace, @UserId, @FullName, @StatusCode);
                    ";

                    var log = new
                    {
                        Date = new EpochDateTime(DateTime.UtcNow.ToUnixTimeMilliseconds()).Value,
                        Message = exception?.InnerException?.Message ?? exception?.Message,
                        Method = context.Request.Method,
                        UserAgent = context.Request.Headers.UserAgent.ToString(),
                        RemoteAddr = context.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
                        RequestPath = context.Request.Path.ToString(),
                        RequestBody = requestBody,
                        StackTrace = exception?.InnerException?.StackTrace ?? exception?.StackTrace,
                        StatusCode = statusCode,
                        UserId = auth?.User?.UserID,
                        FullName = auth?.User?.FullName
                    };

                    await logExecutor.ExecuteAsync(sql, log, commandType: System.Data.CommandType.Text);
                }
            }
            catch { }

            var result = JsonConvert.SerializeObject(new
            {
                Code = statusCode,
                Status = "Invalid",
                Message = exception?.InnerException?.Message ?? exception.Message
            });

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(result);
        }));
    }

    private static async Task<Dictionary<string, string>> ParseMultipartFormAsync(HttpRequest request)
    {
        var formFields = new Dictionary<string, string>();

        var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(request.ContentType).Boundary).Value;

        var reader = new MultipartReader(boundary, request.Body);
        MultipartSection section;

        while ((section = await reader.ReadNextSectionAsync()) != null)
        {
            var contentDisposition = section.GetContentDispositionHeader();

            if (contentDisposition.IsFormDisposition())
            {
                var key = contentDisposition.Name.Value;
                using var streamReader = new StreamReader(section.Body, Encoding.UTF8);
                var value = await streamReader.ReadToEndAsync();
                formFields[key] = value;
            }
            else if (contentDisposition.IsFileDisposition())
            {
                var key = contentDisposition.Name.Value;
                var fileName = contentDisposition.FileName.Value;
                formFields[$"{key}_FileName"] = fileName;
                // Optionally add: contentDisposition.FileNameStar, section.Headers.ContentType, etc.
            }
        }

        return formFields;
    }
}

