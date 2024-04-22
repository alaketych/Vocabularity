namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Rpc;

public interface IRpcRetrieverDataProviderConfig
{
    string RetrieverQueueName { get; }

    int RetrieverTimeoutSeconds { get; }

    int PrefetchMessagesCount { get; }
}