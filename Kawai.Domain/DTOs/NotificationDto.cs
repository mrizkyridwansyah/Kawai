namespace Kawai.Domain.DTOs;

public class NotificationDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string NotifType { get; set; }
    public string Priority { get; set; }
    public string Receiver { get; set; }
    public string Sender { get; set; }
    public bool HasSeen { get; set; }
    public string TimeAgo { get; set; }
}
