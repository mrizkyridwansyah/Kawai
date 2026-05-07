using Kawai.Api;
using System.Text;
using System.Threading.RateLimiting;

public static class RateLimiterExtension
{
    public static string GetHashedToken(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader))
            return "anonymous";

        if (authHeader.StartsWith("Basic "))
        {
            var base64 = authHeader["Basic ".Length..];
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

            // format: username:password
            var username = decoded.Split(':')[0];

            return Cryptography.SHA256Hash(username);
        }

        return Cryptography.SHA256Hash(authHeader);
    }

    public static TokenBucketRateLimiterOptions DefaultLimiterOptions => new()
    {
        TokenLimit = 300,
        TokensPerPeriod = 300,
        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 100,
        AutoReplenishment = true
    };
}
