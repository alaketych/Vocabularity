using Vocabularity.Core.MessageBus.DataProvider.Entities;

namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces;

public interface IBaseRetrieverDataProvider<TMessageRetrieve>
    where TMessageRetrieve : BaseRetrieve
{
    Task InitializeChannel();

    Task<TMessageRetrieve> RetrieveBlockingAsync();

    Task AcknowledgeMessage(ulong broadcastMessageDeliveryTag);
}