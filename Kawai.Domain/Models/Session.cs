namespace Kawai.Api.Models;

public class Session
{
    public string UserId { get; set; }

    public long Date { get; set; }

    public long? ExpiryDate { get; set; }

    public string UserAgent { get; set; }

    public string RemoteAddr { get; set; }

    public string Token { get; set; }
}
