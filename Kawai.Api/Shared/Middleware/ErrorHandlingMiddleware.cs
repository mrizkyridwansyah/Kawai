using Kawai.Data;
using Newtonsoft.Json;
using System.Net;

namespace Kawai.Api.Shared.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Caught early: " + ex.GetType().FullName + " - " + ex.Message);
            throw;
            
            //await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        string message = GetInnermostMessage(ex);
        int statusCode = ex is HttpCustomException custom ? custom.StatusCode : (int)HttpStatusCode.InternalServerError;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var result = JsonConvert.SerializeObject(new
        {
            Code = statusCode,
            Status = "Invalid",
            Message = message
        });

        await context.Response.WriteAsync(result);
    }

    private static string GetInnermostMessage(Exception ex)
    {
        while (ex.InnerException != null)
            ex = ex.InnerException;
        return ex.Message;
    }
}
