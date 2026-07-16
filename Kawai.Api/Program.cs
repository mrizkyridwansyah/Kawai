using AspNetCore.Scalar;
using Dapper;
using Kawai.Api;
using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Api.Shared.Extensions;
using Kawai.Api.Shared.Handlers;
using Kawai.Api.Shared.Middleware;
using Kawai.Api.Services.Logging;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;


Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{DateTime.Now:yy_MM_dd_HH}.txt"));

var config = builder.Configuration;

// Initialize dynamic cryptography keys from configuration if present
Cryptography.Initialize(config["Security:DefaultKey"], config["Security:DefaultSalt"]);
SecurityExtension.Initialize(
    config["Security:ExtraKeySuffix"],
    !string.IsNullOrEmpty(config["Security:SecuritySalt"]) ? Encoding.UTF8.GetBytes(config["Security:SecuritySalt"]) : null
);

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
builder.Services.AddScoped<RazorViewRenderer>();

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<DbExecutor>();
builder.Services.AddScoped<LogExecutor>();
builder.Services.AddSingleton<Kawai.Api.Services.PlaywrightBrowserService>();
builder.Services.AddScoped<DataLogger>();

// Logging buffer: singleton buffer + background flush service
builder.Services.AddSingleton<LogBufferService>();
builder.Services.AddHostedService<LogFlushBackgroundService>();
builder.Services.AddRepositoriesAuto();
builder.Services.AddScoped<IRobotService, RobotService>();
builder.Services.AddScoped<IExportService, ExportService>();


//ini daftarin producer rabbitmq, buat publish message ke queueing => transaksi yg manipulasi stock (receipt, consume, transfer, production, split, dll)
builder.Services.AddSingleton<ITransactionProducer, TransactionProducer>();

//builder.Services.AddSingleton<StockCalculation>();
var handlers = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => typeof(ITransactionHandler).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);

foreach (var handler in handlers)
{
    builder.Services.AddScoped(typeof(ITransactionHandler), handler);
}

//ini daftarin consumer rabbitmq, buat consume message di queueing =>  transaksi yg manipulasi stock (receipt, consume, transfer, production, split, dll)
builder.Services.AddHostedService<TransactionConsumerAsync>();

// Add services to the container.
builder.Services.AddApplication(config);
builder.Services
    .AddAuthentication("Bearer")
    .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>("Bearer", null);

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PerUserTokenPolicy", context =>
    {
        var token = RateLimiterExtension.GetHashedToken(context);
        return RateLimitPartition.GetTokenBucketLimiter(token, _ => RateLimiterExtension.DefaultLimiterOptions);
    });
});

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

//ini tambah signalr buat notif
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers(options =>
{
    // global filter validasi parameter body disini
    options.Filters.Add<ValidateModelAttribute>();
}).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    opt.JsonSerializerOptions.Converters.Add(new EpochDateTimeConverter());
    opt.JsonSerializerOptions.Converters.Add(new JsonTrimString());
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("Kawai.Api"))
    // ini kalo mau tracing secara detail, bisa pake Jaeger (opensource)
    //.WithTracing(tracing =>
    //{
    //    tracing
    //        .AddHttpClientInstrumentation()
    //        .AddAspNetCoreInstrumentation();
    //})
    .WithMetrics(metrics =>
    {
        metrics
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyDotNetApp", serviceVersion: "1.0.0"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            //.AddPrometheusExporter()
            .AddOtlpExporter(otlpOptions =>
            {
                string? uriString = builder.Configuration.GetSection("Monitoring:OpenTelemetry").Get<string>();
                otlpOptions.Endpoint = new Uri(uriString ?? "http://localhost:4318"); // default OTLP/HTTP endpoint
            });
    });

var robotUriString = builder.Configuration["AMR:URI"];
if (string.IsNullOrWhiteSpace(robotUriString))
    throw new InvalidOperationException("The 'AMR:URI' configuration value is missing or empty.");

var username = builder.Configuration["AMR:Auth:Username"];
var password = builder.Configuration["AMR:Auth:Password"];

if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    throw new InvalidOperationException("AMR username/password is missing.");

builder.Services.AddHttpClient("robot", c =>
{
    c.BaseAddress = new Uri(robotUriString);
    c.Timeout = TimeSpan.FromSeconds(10);

    var authToken = Convert.ToBase64String(
        Encoding.ASCII.GetBytes($"{username}:{password}")
    );

    c.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Basic", authToken);
});

builder.Services.AddHttpClient("robot-sync", c =>
{
    c.BaseAddress = new Uri(robotUriString);
    c.Timeout = TimeSpan.FromSeconds(60); // Timeout lebih panjang untuk request synchronous

    var authToken = Convert.ToBase64String(
        Encoding.ASCII.GetBytes($"{username}:{password}")
    );

    c.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Basic", authToken);
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

// init buat trim leading & trailing spasi dan tab di STRING, karna di DB BANYAK pake tipe data CHAR.
SqlMapper.AddTypeHandler(typeof(string), new TrimString());

var app = builder.Build();

// ini buat pake stok mutation secara fifo.
//var stockCalc = app.Services.GetRequiredService<StockCalculation>();
//stockCalc.StartTimer();

using (var scope = app.Services.CreateScope())
{
    var sessionManager = scope.ServiceProvider.GetRequiredService<ISessionManager>();
    SessionManagerAccessor.Instance = sessionManager;
}

app.UseSwagger();
app.UseScalar(options =>
{
    options.UseTheme(Theme.Default);
});

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMorphErrorHandler();
app.UseApplication();
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

app.MapHub<NotifApprovalHub>("/notifapprovalhub");

app.MapRazorPages();
app.Run();

public partial class Program { }