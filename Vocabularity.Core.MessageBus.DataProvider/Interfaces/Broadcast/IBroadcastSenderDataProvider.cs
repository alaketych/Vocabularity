using System;
using System.Threading.Tasks;

namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Broadcast;

public interface IBroadcastSenderDataProvider : IDisposable
{
    Task EnsureDataProviderInitialized();

    Task SendAsync<TRequest>(string correlationId, string routingKey, TRequest request);
}