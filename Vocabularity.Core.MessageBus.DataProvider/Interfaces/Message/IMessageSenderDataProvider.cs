namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Message;

public interface IMessageSenderDataProvider
{
    Task EnsureDataProviderInitialized();

    Task SendAsync<TRequest>(string correlationId, TRequest request);
}