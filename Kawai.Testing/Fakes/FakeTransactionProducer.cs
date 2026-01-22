using Kawai.Api.Services;
using Kawai.Domain.Models;

namespace Kawai.Testing.Fakes;

public class FakeTransactionProducer : ITransactionProducer
{
    public List<object> PublishedMessages { get; } = new();

    public void Publish<T>(StockTransactionMessage<T> message)
    {
        PublishedMessages.Add(message!);
    }

    public void Clear()
    {
        PublishedMessages.Clear();
    }
}

