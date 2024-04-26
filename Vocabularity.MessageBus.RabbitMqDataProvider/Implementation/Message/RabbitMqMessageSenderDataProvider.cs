using RabbitMQ.Client;

using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Message;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Message;
using Vocabularity.MessageBus.RabbitMqDataProvider.Implementation;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Messsage;
public class RabbitMqMessageSenderDataProvider : BaseRabbitMqSenderDataProvider, IMessageSenderDataProvider
{
    private const string SenderMessageConnectionNameTemplate = "Sender_Message: {0}";

    private const bool IsQueueDurable = true;

    private const bool IsQueueExclusive = false;

    private const bool IsQueueAutoDeletable = false;

    private readonly string senderQueueName;

    public RabbitMqMessageSenderDataProvider(
        ISenderRabbitMqConnectionProvider connectionProvider,
        IMessageSenderDataProviderConfig messageSenderdataProviderConfig)
        : base(connectionProvider)
    {
        senderQueueName = messageSenderdataProviderConfig.SenderQueueName;
    }

    public async Task EnsureDataProviderInitialized()
    {
        await EnsureDataProviderInitialized(chanel => { EnsureMessageQueueExists(chanel, this.senderQueueName); });
    }

    public async Task SendAsync<TRequest>(string correlationId, TRequest request)
    {
        await SendAsync(correlationId, request, RabbitMqSenderDataProviderConstants.DirectExchange, this.senderQueueName);
    }

    internal static void EnsureMessageQueueExists(IModel chanel, string messageQueueName)
    {
        // ensure queue exists
        chanel.QueueDeclare(messageQueueName, durable: IsQueueDurable, exclusive: IsQueueExclusive, autoDelete: IsQueueAutoDeletable);
    }

    protected override string GetConnectionName() => string.Format(SenderMessageConnectionNameTemplate, this.senderQueueName);
}