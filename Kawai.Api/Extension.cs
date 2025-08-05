using Kawai.Data;
using Kawai.Data.SqlConnections;
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
            var exception = exceptionHandlerPathFeature.Error;

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
                context.Request.Body.Position = 0;

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

                context.Request.Body.Position = 0;
            }
            catch
            {
                requestBody = "[Failed to read request body]";
            }

            try
            {
                var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
                var auth = context.RequestServices.GetRequiredService<Auth>();
                var logExecutor = context.RequestServices.GetRequiredService<LogExecutor>();

                var sql = @"
                    INSERT INTO ErrorLogs
                    (Date, Message, Method, UserAgent, RemoteAddr, RequestPath, RequestBody, StackTrace, UserId, FullName, StatusCode)
                    VALUES
                    (@Date, @Message, @Method, @UserAgent, @RemoteAddr, @RequestPath, @RequestBody, @StackTrace, @UserId, @FullName, @StatusCode);
                ";

                var log = new
                {
                    Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), // Replace with EpochDateTime.Now if needed
                    Message = exception?.InnerException?.Message ?? exception?.Message,
                    context.Request.Method,
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    RemoteAddr = context.Connection.RemoteIpAddress?.MapToIPv4().ToString(),
                    RequestPath = context.Request.Path.ToString(),
                    RequestBody = requestBody,
                    StackTrace = exception?.InnerException?.StackTrace ?? exception?.StackTrace,
                    context.Response.StatusCode,
                    auth?.User?.UserID,
                    auth?.User?.FullName
                };

                await logExecutor.ExecuteAsync(sql, log, commandType: System.Data.CommandType.Text);
            }
            catch { }

            var result = JsonConvert.SerializeObject(new
            {
                Code = statusCode,
                Status = "Invalid",
                Message = exception?.InnerException?.Message ?? exception.Message,
                //StackTrace = exception?.InnerException?.StackTrace.Split(Environment.NewLine) ?? exception.StackTrace.Split(Environment.NewLine),
                //Errors = new { }
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

