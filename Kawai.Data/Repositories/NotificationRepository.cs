using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;

namespace Kawai.Data.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly DbExecutor _dbExecutor;

    public NotificationRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<int> UnreadNotifByReceiver(string receiver)
    {
        string sp = "sp_Wms_Notifications_CountUnread";
        return await _dbExecutor.QueryFirstOrDefaultAsync<int>(sp, new { Receiver = receiver });
    }

    public async Task<List<NotificationDto>> GetAllNotifByUser(string receiver)
    {
        string sp = "sp_Wms_Notifications_Get";
        return (await _dbExecutor.QueryListAsync<NotificationDto>(sp, new { Receiver = receiver })).ToList();
    }

    public async Task UpdateSeenNotification(long id)
    {
        string sql = @"sp_Wms_Notifications_UpdateSeen";
        int i = await _dbExecutor.ExecuteAsync(sql, new { Id = id});
    }

    public async Task SaveNotification(Notification notification)
    {
        string sql = @"sp_Wms_Notifications_SaveToUser";
        int i = await _dbExecutor.ExecuteAsync(sql, notification);
    }

    public async Task SaveNotificationToAll(Notification notification)
    {
        string sql = @"sp_Wms_Notifications_SaveToAll";
        int i = await _dbExecutor.ExecuteAsync(sql, notification);
    }
}
