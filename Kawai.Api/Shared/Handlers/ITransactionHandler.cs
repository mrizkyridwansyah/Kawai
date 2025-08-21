namespace Kawai.Api.Shared.Handlers;

public interface ITransactionHandler
{
    string TransactionType { get; }
    Task HandleAsync(object payload, string userId);
}
