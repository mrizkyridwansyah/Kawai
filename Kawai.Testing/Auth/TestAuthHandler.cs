using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Kawai.Testing.Auth;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Kalau ada header ini, maka return 401
        if (Request.Headers.ContainsKey("ForceUnauthorized"))
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        
        var claims = new[] {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("UserID", "testuser")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
