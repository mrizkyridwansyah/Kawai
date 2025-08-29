using AspNetCore.Scalar;
using Dapper;
using Kawai.Api;
using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Api.Shared.Extensions;
using Kawai.Api.Shared.Handlers;
using Kawai.Api.Shared.Middleware;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authentication;
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


builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<DbExecutor>();
builder.Services.AddScoped<LogExecutor>();
builder.Services.AddScoped<DataLogger>();
builder.Services.AddRepositoriesAuto();


//ini daftarin producer rabbitmq, buat publish message ke queueing => transaksi yg manipulasi stock (receipt, consume, transfer, production, split, dll)
builder.Services.AddSingleton<ITransactionProducer, TransactionProducer>();

//builder.Services.AddSingleton<StockCalculation>();
builder.Services.AddScoped<ITransactionHandler, ReceiptTransactionHandler>();
builder.Services.AddScoped<ITransactionHandler, MobileReceiptVerifyTransactionHandler>();
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

        var authHeader = context.Request.Headers.Authorization.ToString();
        var token = authHeader.StartsWith("Bearer ")
            ? authHeader["Bearer ".Length..]
            : authHeader;

        if (string.IsNullOrEmpty(token))
        {
            // Fallback ke shared token (atau bisa ditolak)
            token = "anonymous";
        }

        token = Cryptography.SHA256Hash(token);

        // Limit: 5 request per 10 detik per token
        return RateLimitPartition.GetTokenBucketLimiter(token, key => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 5,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
            TokensPerPeriod = 5,
            AutoReplenishment = true
        });
    });

    // Optional: custom response kalau limit terlewati
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Terlalu banyak permintaan. Coba lagi sebentar.", token);
    };
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
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
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
app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseWebSockets();
app.MapControllers();

app.MapHub<NotifApprovalHub>("/notifapprovalhub");

app.MapRazorPages();
app.Run();

