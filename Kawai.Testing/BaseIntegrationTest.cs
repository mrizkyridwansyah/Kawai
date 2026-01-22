using Kawai.Testing.Factory;
using Kawai.Testing.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Testing;

public abstract class BaseIntegrationTest
    : IClassFixture<AuthenticatedFactory>, IClassFixture<UnauthenticatedFactory>
{
    protected readonly HttpClient Client;
    protected readonly HttpClient ClientNoAuth;
    protected readonly FakeTransactionProducer FakeTransactionProducer;

    protected BaseIntegrationTest(AuthenticatedFactory authFactory, UnauthenticatedFactory noAuthFactory)
    {
        Client = authFactory.CreateAuthenticatedClient();
        ClientNoAuth = noAuthFactory.CreateClient();
        FakeTransactionProducer = authFactory.FakeTransactionProducer;

        // pastikan bersih sebelum tiap test
        FakeTransactionProducer.Clear();
    }
}
