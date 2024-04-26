using Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Helpers;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation;

public abstract class BaseRabbitMqSenderDataProvider : IDisposable
{
    private const bool IsMessagePersistent = true;

    private readonly ISenderRabbitMqConnectionProvider connectionProvider;

    #region Sender data provider initialization

    private readonly SemaphoreSlim lockObj = new SemaphoreSlim(1, 1);

    private bool dataProviderIsInitialized;

    #endregion

    protected BaseRabbitMqSenderDataProvider(ISenderRabbitMqConnectionProvider connectionProvider)
    {
        this.connectionProvider = connectionProvider;
    }

    public void Dispose()
    {
        connectionProvider.Dispose();
    }

    protected async Task EnsureDataProviderInitialized(Action<IModel> initializeDataProvider)
    {
        // asyncronously wait for lock
        await ExecuteWithLockAsync(async () =>
        {
            if (!dataProviderIsInitialized)
            {
                var channel = await connectionProvider.GetChannel(GetConnectionName());

                // initalize data-provider
                initializeDataProvider(channel);

                dataProviderIsInitialized = true;
            }
        });
    }

    protected async Task SendAsync<TRequest>(
        string correlationId,
        TRequest request,
        string senderExchange,
        string senderRoutingKey,
        Action<BasicProperties> initializeRpcMessageBasicProperties = null)
    {
        // create message
        var requestMessageBytes = RabbitMqRpcMessageEncoderHelper.Encode(request);
        var basicProperties = new BasicProperties
        {
            CorrelationId = correlationId,
            Type = request.GetType().FullName,
            Persistent = IsMessagePersistent
        };

        // initalize additional message properties
        initializeRpcMessageBasicProperties?.Invoke(basicProperties);

        await ExecuteWithLockAsync(async () =>
        {
            var channel = await connectionProvider.GetChannel(GetConnectionName());

            // send message into queue
            channel.BasicPublish(
                senderExchange,
                senderRoutingKey,
                basicProperties,
                requestMessageBytes);
        });
    }

    protected abstract string GetConnectionName();

    private async Task ExecuteWithLockAsync(Func<Task> exec)
    {
        await this.lockObj.WaitAsync();

        try
        {
            await exec();
        }
        finally
        {
            this.lockObj.Release();
        }
    }
}