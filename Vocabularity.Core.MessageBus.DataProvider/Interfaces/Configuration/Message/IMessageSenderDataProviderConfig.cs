namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Message
{
    public interface IMessageSenderDataProviderConfig
    {
        string SenderQueueName { get; }
    }
}