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
        TokenLimit = 20,
        TokensPerPeriod = 20,
        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 5,
        AutoReplenishment = true
    };
}
