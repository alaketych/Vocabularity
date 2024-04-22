using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Rpc;

namespace Vocabularity.MessageBus.RetrieverConsole.Configuration;

public class RpcRetrieverDataProviderConfig : IRpcRetrieverDataProviderConfig
{
    private RpcRetrieverDataProviderConfig()
    {
    }

    public string RetrieverQueueName => "sender_all";

    public int RetrieverTimeoutSeconds => 10 * 60;

    public int PrefetchMessagesCount => 1;

    internal static IRpcRetrieverDataProviderConfig Instance { get; } = new RpcRetrieverDataProviderConfig();
}