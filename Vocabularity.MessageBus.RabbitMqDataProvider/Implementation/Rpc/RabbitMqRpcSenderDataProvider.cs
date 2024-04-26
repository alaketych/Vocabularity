using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Rpc;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Rpc;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;
using Vocabularity.MessageBus.RabbitMqDataProvider.Implementation;
using Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Helpers;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Rpc;

public class RabbitMqRpcSenderDataProvider : BaseRabbitMqSenderDataProvider, IRpcSenderDataProvider
{
    internal const string ResponseTypeHeaderName = "responseType";

    private const string SenderRpcConnectionNameTemplate = "Sender_Rpc: {0}";

    #region Log messages

    private const string ConsumerCreatedMessage = "MQ Rpc sender's consumer for recieving responses '{ConsumerTag}' is created.";

    private const string SenderInitializedMessage = "MQ Rpc sender is initalized.";

    private const string SentRpcRequestMessage = "MQ Rpc sender has sent a request-message. UtcDateTime: '{UtcDateTime}'.";

    private const string RpcReplyReceivedMessage = "MQ Rpc sender has received a message-reply. UtcDateTime: '{UtcDateTime}'. CorrelationId: '{CorrelationId}'.";

    #endregion

    private const bool IsSenderQueueDurable = true;

    private const bool IsSenderQueueExclusive = false;

    private const bool IsSenderQueueAutoDeletable = false;

    private const bool IsRecieverQueueDurable = false;

    private const bool IsRecieverQueueExclusive = true;

    private const bool IsRecieverQueueAutoDeletable = true;

    private readonly string senderQueueName;

    private readonly string recieverQueueName;

    private readonly int retrieveMessageTimeoutMilliseconds;

    /// <summary>
    /// Dictionary of recieved results where key is correlation-id and value is TaskCompletionSource-object which handle dedicated threads
    /// </summary>
    private readonly Dictionary<string, TaskCompletionSource<byte[]>> recievedResults = new Dictionary<string, TaskCompletionSource<byte[]>>();

    public RabbitMqRpcSenderDataProvider(ISenderRabbitMqConnectionProvider connectionProvider, IRpcSenderDataProviderConfig rpcDataProviderConfig)
        : base(connectionProvider)
    {
        this.senderQueueName = rpcDataProviderConfig.SenderQueueName;
        this.recieverQueueName = $"{rpcDataProviderConfig.RecieverQueueNamePrefix}{Guid.NewGuid()}";
        this.retrieveMessageTimeoutMilliseconds = rpcDataProviderConfig.RetrieveMessageTimeoutMilliseconds;
    }

    public async Task EnsureDataProviderInitialized()
    {
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(string correlationId, TRequest request)
    {
        // initialize task-completion-source object
        var taskCompletionSource = new TaskCompletionSource<byte[]>();

        lock (this)
        {
            this.recievedResults.Add(correlationId, taskCompletionSource);
        }

        try
        {
            // send message into queue
            await this.SendAsync(
                correlationId,
                request,
                RabbitMqSenderDataProviderConstants.DirectExchange,
                this.senderQueueName);

            // wait until response comes
            await Task.WhenAny(taskCompletionSource.Task, Task.Delay(this.retrieveMessageTimeoutMilliseconds));
        }
        finally
        {
            lock (this)
            {
                this.recievedResults.Remove(correlationId);
            }
        }

        var response = default(TResponse);
        if (taskCompletionSource.Task.IsCompleted)
        {
            // gets response and decodes it
            var responseMessageBytes = taskCompletionSource.Task.Result;
            response = RabbitMqRpcMessageEncoderHelper.Decode<TResponse>(responseMessageBytes);
        }

        return response;
    }

    internal static void EnsureSenderQueueExists(IModel channel, string queueName)
    {
        channel.QueueDeclare(queueName, durable: IsSenderQueueDurable, exclusive: IsSenderQueueExclusive, autoDelete: IsSenderQueueAutoDeletable);
    }

    protected override string GetConnectionName() => string.Format(SenderRpcConnectionNameTemplate, this.senderQueueName);

    private void InitializeReciever(IModel channel)
    {
        // initialize consumer for retrieving messages
        var eventingBasicConsumer = new EventingBasicConsumer(channel);
        eventingBasicConsumer.Received += (sender, e) =>
        {
            try
            {
                var correlationId = e.BasicProperties.CorrelationId;

                lock (this)
                {
                    if (recievedResults.ContainsKey(correlationId))
                    {
                        var taskCompletionSource = recievedResults[correlationId];
                        taskCompletionSource.SetResult(e.Body.ToArray());
                    }
                }

                // else message is simply ignored
            }
            catch (Exception exception)
            {
            }
        };
        var consumerTag = channel.BasicConsume(this.recieverQueueName, true, eventingBasicConsumer);
    }
}
