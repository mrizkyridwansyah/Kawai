using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Kawai.Testing.Auth;
using Kawai.Testing.Fakes;
using Kawai.Api.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kawai.Testing.Factory;

public class AuthenticatedFactory : CustomWebApplicationFactory
{
    public AuthenticatedFactory() : base(false) { }
}

public class UnauthenticatedFactory : CustomWebApplicationFactory
{
    public UnauthenticatedFactory() : base(true) { }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly bool _disableTestAuth;
    public FakeTransactionProducer FakeTransactionProducer { get; private set; } = null!;

    public CustomWebApplicationFactory(bool disableTestAuth = false)
    {
        _disableTestAuth = disableTestAuth;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            if (!_disableTestAuth)
            {
                // Auth handler custom untuk test
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                services.AddAuthorization(options =>
                {
                    options.DefaultPolicy =
                        new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder("Test")
                            .RequireAuthenticatedUser()
                            .Build();
                });

            }

            // REMOVE RabbitMQ producer asli
            services.RemoveAll<ITransactionProducer>();

            // ADD Fake producer
            var fakeProducer = new FakeTransactionProducer();
            services.AddSingleton<ITransactionProducer>(fakeProducer);

            // simpan reference buat test
            FakeTransactionProducer = fakeProducer;

            // kalau _disableTestAuth = true, kita biarkan pipeline asli, jadi 401 asli akan muncul
        });
    }

    // Client biasa, pakai TestAuthHandler
    public HttpClient CreateAuthenticatedClient() => this.CreateClient();

    // Client tanpa TestAuthHandler → bisa nge-test 401 asli
    public HttpClient CreateUnauthenticatedClient()
    {
        var factory = new CustomWebApplicationFactory(disableTestAuth: true);
        return factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
