using System.Threading.Tasks;

using RabbitMQ.Client;

using Vocabularity.Core.MessageBus.DataProvider.Entities;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Broadcast;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Broadcast;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Broadcast;

public class RabbitMqBroadcastRetrieverDataProvider : BaseRabbitMqRetrieverDataProvider, IBroadcastRetrieverDataProvider
{
    private const bool IsRetrieverQueueDurable = true;

    private const bool IsRetrieverQueueExclusive = false;

    private const bool IsRetrieverQueueAutoDeletable = false;

    private readonly string exchangeName;

    private readonly string[] routingKeys;

    public RabbitMqBroadcastRetrieverDataProvider(
        IRetrieverRabbitMqConnectionProvider connectionProvider,
        IBroadcastRetrieverDataProviderConfig retrieverDataProviderConfig)
        : base(
            connectionProvider,
            retrieverDataProviderConfig.RetrieverQueueName,
            retrieverDataProviderConfig.PrefetchMessagesCount,
            retrieverDataProviderConfig.RetrieverTimeoutSeconds)
    {
        this.exchangeName = retrieverDataProviderConfig.ExchangeName;
        this.routingKeys = retrieverDataProviderConfig.RoutingKeys;
    }

    public async Task InitializeChannel()
    {
        await this.InitializeChannel(
            channel =>
            {
                // create reciever queue
                channel.QueueDeclare(
                    this.RetrieverQueueName,
                    durable: IsRetrieverQueueDurable,
                    exclusive: IsRetrieverQueueExclusive,
                    autoDelete: IsRetrieverQueueAutoDeletable);

                // ensure that broadcast exchange-name exists
                RabbitMqBroadcastSenderDataProvider.EnsureBroadcastExchangeExists(channel, this.exchangeName);

                // bind queue to topic by using routing-keys
                foreach (var routingKey in this.routingKeys)
                {
                    channel.QueueBind(this.RetrieverQueueName, this.exchangeName, routingKey);
                }
            });
    }

    public async Task<BroadcastRetrieve> RetrieveBlockingAsync()
    {
        var broadCastRetrieveMessage = await this.RetrieveBlockingAsync(
                                           basicDeliverEventArgs => GetRetrieveMessage<BroadcastRetrieve>(
                                               basicDeliverEventArgs.BasicProperties.Type,
                                               basicDeliverEventArgs.Body,
                                               basicDeliverEventArgs.BasicProperties.CorrelationId,
                                               retrieveMessage =>
                                               {
                                                   // set delivery-tag property
                                                   retrieveMessage.DeliveryTag = basicDeliverEventArgs.DeliveryTag;
                                               }));
        return broadCastRetrieveMessage;
    }

    public async Task AcknowledgeMessage(ulong broadcastMessageDeliveryTag)
    {
        await Task.Run(() => { this.AcknowledgeMessageImplementation(broadcastMessageDeliveryTag); });
    }
}