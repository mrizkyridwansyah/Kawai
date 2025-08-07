using Kawai.Domain.DTOs;
using Kawai.Domain.Models;

namespace Kawai.Domain.Interfaces;

public interface INotificationRepository
{
    Task<int> UnreadNotifByReceiver(string receiver);
    Task<List<NotificationDto>> GetAllNotifByUser(string receiver);
    Task SaveNotification(Notification notification);
}
