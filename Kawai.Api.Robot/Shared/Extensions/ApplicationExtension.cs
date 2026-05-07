using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Kawai.Api.Robot;

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
        services.AddHealthChecks();
        services.AddSwagger();
    }

    static void AddSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                Description = "Basic auth"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
