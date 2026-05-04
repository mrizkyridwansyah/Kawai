using AspNetCore.Scalar;
using Dapper;
using Kawai.Api.Robot;
using Kawai.Api.Robot.Services;
using Kawai.Data.Repositories.Robot;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using Prometheus;
using System.Threading.RateLimiting;


Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{DateTime.Now:yy_MM_dd_HH}.txt"));

var config = builder.Configuration;

// CORS
// tadi nya pake ini, tapi signalr nya error
//builder.Services.AddCors(confg =>
//                confg.AddPolicy("AllowAll",
//                    p => p.AllowAnyOrigin()
//                        .AllowAnyMethod()
//                    .AllowAnyHeader()));

// akhir nya pake ini
var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(config =>
    config.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
    ));


builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<DbExecutor>();
builder.Services.AddScoped<LogExecutor>();
builder.Services.AddScoped<DataLogger>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PerUserTokenPolicy", context =>
    {
        var token = RateLimiterExtension.GetHashedToken(context);
        return RateLimitPartition.GetTokenBucketLimiter(token, _ => RateLimiterExtension.DefaultLimiterOptions);
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

//ini tambah signalr buat notif
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(r => r.AddService("Kawai.Api.Robot"))
//    // ini kalo mau tracing secara detail, bisa pake Jaeger (opensource)
//    //.WithTracing(tracing =>
//    //{
//    //    tracing
//    //        .AddHttpClientInstrumentation()
//    //        .AddAspNetCoreInstrumentation();
//    //})
//    .WithMetrics(metrics =>
//    {
//        metrics
//            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyDotNetApp", serviceVersion: "1.0.0"))
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddRuntimeInstrumentation()
//            //.AddPrometheusExporter()
//            .AddOtlpExporter(otlpOptions =>
//            {
//                string? uriString = builder.Configuration.GetSection("Monitoring:OpenTelemetry").Get<string>();
//                otlpOptions.Endpoint = new Uri(uriString ?? "http://localhost:4318"); // default OTLP/HTTP endpoint
//            });
//    });

//builder.Services.AddSingleton<IApiKeyRepository, ApiKeyRepository>();
//builder.Services
//    .AddAuthentication("HmacScheme")
//    .AddScheme<AuthenticationSchemeOptions, HmacAuthenticationHandler>(
//        "HmacScheme", null);

builder.Services.AddScoped<IRobotRepository, RobotRepository>();

builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

// init buat trim leading & trailing spasi dan tab di STRING, karna di DB BANYAK pake tipe data CHAR.
SqlMapper.AddTypeHandler(typeof(string), new TrimString());

var app = builder.Build();


app.UseSwagger();
app.UseScalar(options =>
{
    options.UseTheme(Theme.Default);
});

//app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMorphErrorHandler();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpMetrics();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapMetrics();
});

app.UseRateLimiter(new RateLimiterOptions
{
    GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var token = RateLimiterExtension.GetHashedToken(context);
        return RateLimitPartition.GetTokenBucketLimiter(token, _ => RateLimiterExtension.DefaultLimiterOptions);
    }),
    OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        var responseObj = new
        {
            Code = 429,
            Status = "Too Many Requests",
            Message = "Terlalu banyak permintaan."
        };

        var json = System.Text.Json.JsonSerializer.Serialize(responseObj);

        await context.HttpContext.Response.WriteAsync(json, cancellationToken);
    }
});

app.UseWebSockets();
app.MapControllers();

app.MapRazorPages();
app.Run();

public partial class Program { }