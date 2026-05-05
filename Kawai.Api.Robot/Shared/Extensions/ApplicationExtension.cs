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
    }
}
