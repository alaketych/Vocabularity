using RabbitMQ.Client;

using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Broadcast;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Broadcast;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Broadcast;

public class RabbitMqBroadcastSenderDataProvider : BaseRabbitMqSenderDataProvider, IBroadcastSenderDataProvider
{
    private const string SenderBroadcastConnectionNameTemplate = "Sender_Broadcast: {0}";

    private const string ExchangeType = "topic";

    private const bool IsExchangeDurable = true;

    private readonly string exchangeName;

    public RabbitMqBroadcastSenderDataProvider(
        ISenderRabbitMqConnectionProvider connectionProvider,
        IBroadcastSenderDataProviderConfig broadcastSenderdataProviderConfig)
        : base(connectionProvider)
    {
        this.exchangeName = broadcastSenderdataProviderConfig.ExchangeName;
    }

    public async Task EnsureDataProviderInitialized()
    {
        await EnsureDataProviderInitialized(chanel => { EnsureBroadcastExchangeExists(chanel, this.exchangeName); });
    }

    public async Task SendAsync<TRequest>(string correlationId, string routingKey, TRequest request)
    {
        await SendAsync(correlationId, request, this.exchangeName, routingKey);
    }

    internal static void EnsureBroadcastExchangeExists(IModel chanel, string broadCastExchangeName)
    {
        // ensure exchange exists
        chanel.ExchangeDeclare(broadCastExchangeName, ExchangeType, durable: IsExchangeDurable);
    }

    protected override string GetConnectionName() => string.Format(SenderBroadcastConnectionNameTemplate, this.exchangeName);
}