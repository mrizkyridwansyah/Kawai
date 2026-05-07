using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kawai.Api.Robot.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _config;

        private string? _failureMessage;

        public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration config)
        : base(options, logger, encoder, clock)
        {
            _config = config;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                _failureMessage = "Missing Authorization Header";
                return Task.FromResult(AuthenticateResult.Fail(_failureMessage));
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

                var username = credentials[0];
                var password = credentials[1];

                var configUsername = _config.GetSection("BasicAuthAMR:Username").Get<string[]>();
                var configPassword = _config["BasicAuthAMR:Password"];

                //setting bebas
                if (configUsername == null || !configUsername.Contains(username) || password != configPassword)
                {
                    _failureMessage = "Invalid Username or Password";
                    return Task.FromResult(AuthenticateResult.Fail(_failureMessage));
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Robot")
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                _failureMessage = "Invalid Authorization Header";
                return Task.FromResult(AuthenticateResult.Fail(_failureMessage));
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;

            await Response.WriteAsJsonAsync(new
            {
                status = 401,
                error = "Unauthorized",
                message = _failureMessage ?? "Unauthorized"
            });
        }
    }
}
