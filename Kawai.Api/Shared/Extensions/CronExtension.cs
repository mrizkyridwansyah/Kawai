using Kawai.Api.Services;
using Kawai.Api.Shared;
using System.Reflection;

namespace Kawai.Api;

public static class CronExtensions
{
    public static void AddCronJobs(this IServiceCollection services)
    {
        services.AddScoped<CronManager>();
        var list = typeof(CronJob).Assembly.ExportedTypes.Where(p => p.GetCustomAttribute<CronJob>() != null && p.BaseType == typeof(BaseCronJob)).ToList();
        foreach (var r in list)
        {
            services.AddScoped(r);
        }
    }

    public static void UseCronJobs(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var cronManager = scope.ServiceProvider.GetService<CronManager>();
        cronManager.RunJobs();
    }
}
