using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using System.Diagnostics;

namespace Kawai.Api.CronJobs;

public class StockCalculation
{
    private readonly IServiceScopeFactory _scopeFactory;
    private int transactionCount = 0;
    private const int MaxTransactions = 2;
    private readonly object lockObj = new();
    private bool isRunning = false;

    public StockCalculation(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void AddTransaction()
    {
        lock (lockObj)
        {
            if (transactionCount < MaxTransactions)
            {
                transactionCount++;
                Console.WriteLine($"Transaction added. Count: {transactionCount}");
            }
            else
            {
                Console.WriteLine("Transaction limit reached.");
            }
        }
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            _ = RunTimerLoopAsync();
        }
    }

    private async Task RunTimerLoopAsync()
    {
        while (true)
        {
            await Task.Delay(2000); // Timer every 2s

            while (true)
            {
                lock (lockObj)
                {
                    if (transactionCount <= 0)
                        break;

                    transactionCount--; // reserve slot
                }

                Console.WriteLine("Start Calculate Mutation...");
                var stopwatch = Stopwatch.StartNew();

                await CalculateMutation();
                stopwatch.Stop();

                Console.WriteLine("Done Calculate Mutation");
                await Task.Delay((int)(stopwatch.ElapsedMilliseconds / 1000));

                await CalculateMutation();
            }
        }

    }

    private async Task CalculateMutation()
    {
        using var scope = _scopeFactory.CreateScope();
        var _stockRepository = scope.ServiceProvider.GetRequiredService<IStockMutationRepository>();
        var _notifRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        var pendingMutations = await _stockRepository.GetPendingStock();
        Console.WriteLine("Pending Stock Mutation: " + pendingMutations.Count.ToString());
        if (pendingMutations?.Any() != true) return;

        var mutationGroups = pendingMutations
            .GroupBy(m => new { m.SourceType, m.SourceRef })
            .Select(g => new
            {
                g.Key.SourceType,
                g.Key.SourceRef,
                MinId = g.Min(x => x.Id)  // buat jaga urutan original
            })
            .OrderBy(g => g.MinId)       // Urutkan berdasarkan ID stockmutation terkecil
            .ToList();

        foreach (var group in mutationGroups)
        {
            var sourceType = group.SourceType.ToUpper();
            var sourceRef = group.SourceRef;
            var mutation = pendingMutations.FirstOrDefault(p => p.SourceRef == sourceRef);

            Notification notification = new Notification
            {
                Title = $"Transaction {sourceType}",
                Priority = "TOP",
                Receiver = mutation.RegisterUser,
                Sender = mutation.RegisterUser,
                UrlRedirect = ""
            };

            try
            {
                switch (sourceType)
                {
                    case "RECEIPT":
                        await _stockRepository.Receipt(sourceRef);
                        break;
                    case "PRODUCTION":
                        await _stockRepository.Production(sourceRef);
                        break;
                    case "ADJUSTMENT":
                        await _stockRepository.Adjustment(sourceRef);
                        break;
                    case "TRANSFER":
                        await _stockRepository.Transfer(sourceRef);
                        break;
                    case "PREPARE-CONSUME":
                        await _stockRepository.PrepareConsume(sourceRef);
                        break;
                    //case "USED-CONSUME":
                    //    await _stockRepository.UsedConsume(sourceRef);
                    //    break;
                    case "SPLIT":
                        await _stockRepository.Split(sourceRef);
                        break;
                    default:
                        break;
                }

                notification.Description = $"Transaction {sourceType} - {mutation.SourceRefNo} berhasil.";
                notification.NotifType = "INFO";
                _notifRepository.SaveNotification(notification);
                await SendNotification(sourceType, sourceRef, mutation.RegisterUser, [notification]);
            }
            catch (Exception ex)
            {
                notification.Description = $"Transaction {sourceType} Gagal. {Environment.NewLine + ex.Message}";
                notification.NotifType = "ERROR";
                _notifRepository.SaveNotification(notification);
                await SendNotification(sourceType, sourceRef, mutation.RegisterUser, [notification]);
            }

        }
    }

    private async Task SendNotification(string sourceType, string sourceRef, string receiver, List<Notification> notifications)
    {
        using var scope = _scopeFactory.CreateScope();
        var _notificationService = scope.ServiceProvider.GetRequiredService<NotificationService<NotifApprovalHub>>();

        await _notificationService.BroadCastOnlyTo([receiver], "NewNotification", new
        {
            Count = 1,
            Notifications = notifications
        });
    }
}