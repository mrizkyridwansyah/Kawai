using Hangfire;
using Kawai.Api.Shared;
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
public class StockMasterRecalculate: BaseCronJob
{
    private readonly IStockRepository _stockRepository;

    public StockMasterRecalculate(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public override Task Run()
    {
        _stockRepository.RecalculateStockMaster();
        return default;
    }
}
