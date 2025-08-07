using Kawai.Api.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Kawai.Api;

public static class ApplicationExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.SerializerOptions.Converters.Add(new EpochDateTimeConverter());
        });
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddScoped<Auth>();
        services.AddScoped<Smtp>();
        services.AddScoped<SessionManager>();
        services.AddScoped<ISessionManager, SessionManager>();
        services.AddScoped(typeof(NotificationService<>));
        services.AddFileStorage(config);
        services.Configure<MailSettings>(config.GetSection("Mailer:Smtp"));
        services.AddMailer(config);
        services.AddKawaiHangfire();
        services.AddRazorPages();
        services.AddSwagger();
        services.AddCronJobs();
        services.AddHealthChecks();
    }

    static void AddSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.CustomSchemaIds(p => p.FullName);
            opt.MapType<EpochDateTime>(() => new OpenApiSchema { Type = "integer", Format = "int64" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                //BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
    }

    public static void UseApplication(this WebApplication app)
    {
        app.UseKawaiEntity();
        app.UseKawaiHangfire();
        app.UseCronJobs();
        app.UseHealthChecks("/check");
    }

}
