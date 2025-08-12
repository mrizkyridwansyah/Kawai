using Kawai.Api.Shared;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;

namespace Kawai.Api.CronJobs;

/// <summary>
/// https://cron.help/ ini contoh kalo mau tau helper cron expression nya 
/// minimal interval adalah 1 menit
/// </summary>
[CronJob("* * * * *")]
public class Test: BaseCronJob
{
    private readonly IItemRepository _itemRepository;

    public Test(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public override Task Run()
    {
        var result = _itemRepository.GetAll(new Domain.Shared.RequestParameter());  
        Console.WriteLine("Test cron job executed at: " + DateTime.Now);
        Console.WriteLine(string.Join("\n", result.Result.Select(p => $"ItemCode: {p.ItemCode}, ItemName: {p.ItemName}")));



        // Implement your cron job logic here
        return default;
    }
}
