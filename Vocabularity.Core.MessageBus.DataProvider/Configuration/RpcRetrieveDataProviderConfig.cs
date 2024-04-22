using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Rpc;

namespace Vocabularity.Core.MessageBus.DataProvider.Configuration;

public abstract class RpcRetrieverDataProviderConfig : IRpcRetrieverDataProviderConfig
{
    public string SectionName { get; protected set; }

    #region IRpcRetrieverDataProviderConfig interface

    public string RetrieverQueueName { get; set; }

    public int RetrieverTimeoutSeconds { get; set; }

    public int PrefetchMessagesCount { get; set; }

    #endregion
}