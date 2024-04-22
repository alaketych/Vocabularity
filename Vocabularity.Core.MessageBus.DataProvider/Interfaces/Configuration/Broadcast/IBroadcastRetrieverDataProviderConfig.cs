namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Broadcast
{
    public interface IBroadcastRetrieverDataProviderConfig
    {
        string ExchangeName { get; }

        string RetrieverQueueName { get; }

        string[] RoutingKeys { get; }

        int RetrieverTimeoutSeconds { get; }

        int PrefetchMessagesCount { get; }
    }
}