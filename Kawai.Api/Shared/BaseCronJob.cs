namespace Kawai.Api.Shared;

public interface ICronJob { }

/// <summary>
/// Cron in "min hour day month day-of-week" format (e.g., "0 0 * * *" = hourly)
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CronJob : Attribute
{
    public string CronExpression { get; set; }
    public CronJob(string cronExpression)
    {
        CronExpression = cronExpression;
    }
}

public abstract class BaseCronJob : ICronJob
{
    public abstract Task Run();
}
