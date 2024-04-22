namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Message
{
    public interface IMessageRetrieverDataProviderConfig
    {
        string RetrieverQueueName { get; }

        int RetrieverTimeoutSeconds { get; }

        int PrefetchMessagesCount { get; }
    }
}