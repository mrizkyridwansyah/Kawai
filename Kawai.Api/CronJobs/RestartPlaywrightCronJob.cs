using Kawai.Api.Services;
using Kawai.Api.Shared;

namespace Kawai.Api.CronJobs;

[CronJob("0 4 * * *")] // Jalan setiap jam 4 pagi
public class RestartPlaywrightCronJob : BaseCronJob
{
    private readonly PlaywrightBrowserService _browserService;

    public RestartPlaywrightCronJob(PlaywrightBrowserService browserService)
    {
        _browserService = browserService;
    }

    public override async Task Run()
    {
        await _browserService.RestartBrowserAsync();
    }
}
