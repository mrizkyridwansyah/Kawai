using AspNetCore.Scalar;
using Dapper;
using Kawai.Api.Robot;
using Kawai.Api.Robot.Services;
using Kawai.Api.Robot.Shared.Middleware;
using Kawai.Data.Repositories.Robot;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;


Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{DateTime.Now:yy_MM_dd_HH}.txt"));

var config = builder.Configuration;

builder.Services.AddCors(confg =>
                confg.AddPolicy("AllowAll",
                   p => p.AllowAnyOrigin()
                        .AllowAnyMethod()
                    .AllowAnyHeader()));

builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<DbExecutor>();
builder.Services.AddScoped<LogExecutor>();
builder.Services.AddScoped<DataLogger>();

builder.Services.AddSingleton<Kawai.Api.Robot.Services.Logging.LogBufferService>();
builder.Services.AddHostedService<Kawai.Api.Robot.Services.Logging.LogFlushBackgroundService>();

builder.Services.AddApplication(config);

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
builder.Services.AddScoped<IRobotRepository, RobotRepository>();

builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

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


app.UseSwagger();
app.UseScalar(options =>
{
    options.UseTheme(Theme.Default);
});

app.UseMorphErrorHandler();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();