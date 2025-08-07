using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Notification
{
    [MaxLength(100)]
    public string Title { get; set; }
    public string Description { get; set; }

    [AllowedValues(["INFO", "WARNING", "ERROR"])]
    public string NotifType { get; set; }

    [AllowedValues(["TOP", "MIDDLE", "LOW"])]
    public string Priority { get; set; }

    public string Receiver { get; set; }
    public string Sender { get; set; }
}
