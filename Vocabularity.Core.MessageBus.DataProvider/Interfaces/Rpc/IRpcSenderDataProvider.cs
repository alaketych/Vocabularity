namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Rpc;

public interface IRpcSenderDataProvider : IDisposable
{
    Task EnsureDataProviderInitialized();

    Task<TResponse> SendAsync<TRequest, TResponse>(string correlationId, TRequest request);
}