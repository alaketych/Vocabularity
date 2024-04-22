using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Rpc;

namespace GroundScope.MessageBus.SenderConsole.Configuration;

internal class RpcSenderDataProviderConfig : IRpcSenderDataProviderConfig
{
    private RpcSenderDataProviderConfig()
    {
    }

    public string SenderQueueName => "BookingsService_Rpc_Requests";

    public string RecieverQueueNamePrefix => "reciever_";

    public int RetrieveMessageTimeoutMilliseconds => 10000 * 1000;

    internal static IRpcSenderDataProviderConfig Instance { get; } = new RpcSenderDataProviderConfig();
}