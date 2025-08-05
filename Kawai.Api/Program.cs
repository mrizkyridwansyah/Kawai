using AspNetCore.Scalar;
using Kawai.Api;
using Kawai.Api.Shared;
using Kawai.Api.Shared.Extensions;
using Kawai.Api.Shared.Middleware;
using Kawai.Data.Repositories;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;

Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{DateTime.Now:yy_MM_dd_HH}.txt"));

var config = builder.Configuration;

// CORS
builder.Services.AddCors(confg =>
                confg.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin()
                        .AllowAnyMethod()
                    .AllowAnyHeader()));


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

var app = builder.Build();

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
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();
app.MapRazorPages();
app.Run();
