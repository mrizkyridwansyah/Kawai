using Kawai.Api;
using System.Threading.RateLimiting;

public static class RateLimiterExtension
{
    public static string GetHashedToken(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        var token = authHeader.StartsWith("Bearer ")
            ? authHeader["Bearer ".Length..]
            : authHeader;

        return string.IsNullOrEmpty(token) ? "anonymous" : Cryptography.SHA256Hash(token);
    }

    public static TokenBucketRateLimiterOptions DefaultLimiterOptions => new()
    {
        TokenLimit = 50,
        TokensPerPeriod = 50,
        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 20,
        AutoReplenishment = true
    };
}
