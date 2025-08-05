using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;

namespace Kawai.Api;

public class AuthenticationScheme
{
    public const string Bearer = "BEARER";
    public const string Query = "QUERY";
    public const string Cookies = "COOKIES";
}

public class BearerAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration,
    SessionManager sessionManager,
    IHttpContextAccessor httpContextAccessor) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected IConfiguration Configuration { get; } = configuration;
    protected HttpContext HttpContext { get; } = httpContextAccessor.HttpContext;
    protected SessionManager SessionManager { get; } = sessionManager;

    private static readonly string[] roles = ["test"];

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string values = "";

        string token = "";
        var isCookie = Request.Cookies.TryGetValue("__SIDX", out values);
        var isBearer = Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value);

        if (!isCookie && !isBearer)
            return AuthenticateResult.Fail("Unauthorized");

        if (isCookie)
            token = values;

        if (isBearer)
        {
            string authorizationHeader = string.IsNullOrWhiteSpace(value) ? values : value;
            if (string.IsNullOrEmpty(authorizationHeader))
                return AuthenticateResult.NoResult();

            if (!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Unauthorized");

            token = authorizationHeader["bearer".Length..].Trim();
        }

        if (string.IsNullOrEmpty(token))
            return AuthenticateResult.Fail("Unauthorized");

        if (!SessionManager.ValidateToken(token).Result.IsSucceeded)
            return AuthenticateResult.Fail("Unauthorized");

        var claims = new List<Claim> { };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new GenericPrincipal(identity, roles);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        await Task.CompletedTask;
        return AuthenticateResult.Success(ticket);
    }
}

public class QueryAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    protected IConfiguration Configuration { get; }
    protected HttpContext HttpContext { get; }
    protected List<string> SecretKeys { get; }

    private static readonly string[] roles = ["test"];

    [Obsolete]
    public QueryAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
        : base(options, logger, encoder, clock)
    {
        Configuration = configuration;
        HttpContext = httpContextAccessor.HttpContext;
        SecretKeys = Configuration.GetSection("SecretKeys").AsEnumerable().Select(p => p.Value).ToList();
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Query.ContainsKey("token"))
            return AuthenticateResult.Fail("Unauthorized");

        string token = Request.Query["token"];

        if (string.IsNullOrEmpty(token))
            return AuthenticateResult.Fail("Unauthorized");

        if (!SecretKeys.Any(p => p == token))
            return AuthenticateResult.Fail("Unauthorized");

        var claims = new List<Claim> { };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new GenericPrincipal(identity, roles);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        await Task.CompletedTask;
        return AuthenticateResult.Success(ticket);
    }
}
