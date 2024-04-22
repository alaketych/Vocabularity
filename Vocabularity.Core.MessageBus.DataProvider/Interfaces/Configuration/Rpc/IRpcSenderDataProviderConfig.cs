namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Rpc
{
    public interface IRpcSenderDataProviderConfig
    {
        string SenderQueueName { get; }

        string RecieverQueueNamePrefix { get; }

        int RetrieveMessageTimeoutMilliseconds { get; }
    }
}