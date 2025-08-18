using Hangfire;
using Kawai.Api.Shared;
using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;

namespace Kawai.Api.CronJobs;

/// <summary>
/// https://cron.help/ ini contoh kalo mau tau helper cron expression nya 
/// minimal interval adalah 1 menit
/// </summary>
[CronJob("* * * * *")]
// batas waktu 5 menit, jadi kalo logic nya lebih dari 5 menit, maka job selanjut nya boleh jalan.
// jadi DisableConcurrentExecution itu cuma buat handle jika job sebelumnya belum selesai, maka job selanjutnya tidak akan dijalankan.
// kalo mau jaminan biar ga overlap, maka harus pastikan logic di Run itu tidak lebih dari 5 menit, atau bisa juga di handle dengan cara lain seperti queueing atau locking mechanism atau flagging di db.
[DisableConcurrentExecution(600)]
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
