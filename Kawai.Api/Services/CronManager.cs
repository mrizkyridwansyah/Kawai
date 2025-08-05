using Hangfire;
using Kawai.Api.Shared;
using System.Reflection;

namespace Kawai.Api.Services;

public class CronManager
{
    protected IServiceProvider ServiceProvider { get; }
    protected IRecurringJobManager JobManager { get; }
    public CronManager(IServiceProvider serviceProvider, IRecurringJobManager recurringJob)
    {
        ServiceProvider = serviceProvider;
        JobManager = recurringJob;
    }

    public void RunJobs()
    {
        var list = typeof(CronJob).Assembly.ExportedTypes.Where(p => p.GetCustomAttribute<CronJob>() != null && p.BaseType == typeof(BaseCronJob)).ToList();

        foreach (var r in list)
        {
            var c = ServiceProvider.GetRequiredService(r) as BaseCronJob;
            var expr = c.GetType().GetCustomAttribute<CronJob>();
            JobManager.AddOrUpdate(r.FullName, () => c.Run(), expr.CronExpression, new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time") // GMT+7 (WIB)
            });
        }
    }
}
