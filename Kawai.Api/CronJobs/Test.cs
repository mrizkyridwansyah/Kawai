using Kawai.Api.Shared;

namespace Kawai.Api.CronJobs;

/// <summary>
/// https://cron.help/ ini contoh kalo mau tau helper cron expression nya 
/// minimal interval adalah 1 menit
/// </summary>
[CronJob("* * * * *")]
public class Test: BaseCronJob
{
    public override Task Run()
    {
        Console.WriteLine("Test cron job executed at: " + DateTime.Now);
        // Implement your cron job logic here
        return default;
    }
}
