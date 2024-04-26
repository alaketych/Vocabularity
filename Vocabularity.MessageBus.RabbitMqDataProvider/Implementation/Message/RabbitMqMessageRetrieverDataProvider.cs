using Vocabularity.Core.MessageBus.DataProvider.Entities;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Message;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Message;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Messsage;

public class RabbitMqMessageRetrieverDataProvider : BaseRabbitMqRetrieverDataProvider, IMessageRetrieverDataProvider
{
    public RabbitMqMessageRetrieverDataProvider(
        IRetrieverRabbitMqConnectionProvider connectionProvider,
        IMessageRetrieverDataProviderConfig retrieverDataProviderConfig)
        : base(
            connectionProvider,
            retrieverDataProviderConfig.RetrieverQueueName,
            retrieverDataProviderConfig.PrefetchMessagesCount,
            retrieverDataProviderConfig.RetrieverTimeoutSeconds)
    {
    }

    public async Task InitializeChannel()
    {
        await this.InitializeChannel(
            channel =>
            {
                // create reciever queue
                RabbitMqMessageSenderDataProvider.EnsureMessageQueueExists(channel, this.RetrieverQueueName);
            });
    }

    public async Task<MessageRetrieve> RetrieveBlockingAsync()
    {
        var retrieveMessage = await this.RetrieveBlockingAsync(
                                  basicDeliverEventArgs => GetRetrieveMessage<MessageRetrieve>(
                                      basicDeliverEventArgs.BasicProperties.Type,
                                      basicDeliverEventArgs.Body,
                                      basicDeliverEventArgs.BasicProperties.CorrelationId,
                                      message =>
                                      {
                                          // set delivery-tag property
                                          message.DeliveryTag = basicDeliverEventArgs.DeliveryTag;
                                      }));
        return retrieveMessage;
    }

    public async Task AcknowledgeMessage(ulong queueMessageDeliveryTag)
    {
        await Task.Run(() => { this.AcknowledgeMessageImplementation(queueMessageDeliveryTag); });
    }
}