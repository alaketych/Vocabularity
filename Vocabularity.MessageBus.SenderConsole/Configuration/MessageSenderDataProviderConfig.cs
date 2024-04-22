using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Message;

namespace GroundScope.MessageBus.SenderConsole.Configuration;

internal class MessageSenderDataProviderConfig : IMessageSenderDataProviderConfig
{
    private MessageSenderDataProviderConfig()
    {
    }

    public string SenderQueueName => "";

    // public string SenderQueueName => "InvoicesService_Rpc_Requests";
    public string RecieverQueueNamePrefix => "reciever_";

    public int RetrieveMessageTimeoutMilliseconds => 10000 * 1000;

    internal static IMessageSenderDataProviderConfig Instance { get; } = new MessageSenderDataProviderConfig();
}