using AspNetCore.Scalar;
using Dapper;
using Kawai.Api;
using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Api.Shared;
using Kawai.Api.Shared.Extensions;
using Kawai.Api.Shared.Middleware;
using Kawai.Data.Repositories;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authentication;

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

// Add services to the container.

builder.Services.AddApplication(config);
builder.Services
    .AddAuthentication("Bearer")
    .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>("Bearer", null);
//.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", options => { });

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

//ini tambah signalr buat notif
builder.Services.AddSignalR();

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
app.UseWebSockets();
app.MapControllers();

app.MapHub<NotifApprovalHub>("/notifapprovalhub");

app.MapRazorPages();
app.Run();
