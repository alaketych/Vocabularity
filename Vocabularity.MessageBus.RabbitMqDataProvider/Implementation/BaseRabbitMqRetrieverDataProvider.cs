using System.Reflection;

using Vocabularity.Core.MessageBus.DataProvider.Entities;
using Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Helpers;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation;

public abstract class BaseRabbitMqRetrieverDataProvider : IDisposable
{
    protected static readonly Assembly RpcMessagesAssembly = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == RpcMessagesAssemblyName);

    protected readonly string RetrieverQueueName;

    #region Log-messages

    private const string ConsumerCreatedMessage = "MQ consumer '{ConsumerTag}' (retriever queue name '{RetrieverQueueName}') for retriever is created.";

    private const string ConsumingStoppingMessage = "MQ consumer '{ConsumerTag}' (retriever queue name '{RetrieverQueueName}') is stopping due to retrieving timeout.";

    private const string ConsumerRecreationIsRequested = "MQ consumer '{ConsumerTag}' (retriever queue name '{RetrieverQueueName}') recreation is requested.";

    private const string ConsumerCancelledMessage = "MQ consumer '{ConsumerTag}' (retriever queue name '{RetrieverQueueName}') is cancelled for retriever.";

    private const string MessageReceivedMessage = "MQ message is received. UtcDateTime: '{UtcDateTime}'. CorrelationId: '{CorrelationId}'.";

    private const string MessageAcknowledgedMessage = "MQ message is acknowledged. UtcDateTime: '{UtcDateTime}'.";

    #endregion

    private const string RetrieverConnectionNameTemplate = "Retriever: {0}";

    private const string RpcMessagesAssemblyName = "Vocabularity.Core.MessageBus";

    private const bool IsRetrieveMessageAcknowledgedAutomatically = false;

    private const int PrefetchSize = 0;

    private const bool IsGlobal = false;

    private const int NumberOfInProgressMessagesAvailableForReConnectionOrConsumerReCreation = 0;

    private const int WaitMessagesToBeFinishedSeconds = 10;

    private const string NotPossibleToReconnectDueToMessagesInProgress = "It is required to wait all messages to be processed";

    private const double ConsumerReCreationThresholdMultiplier = 0.9;

    private readonly int retreiverTimeoutSeconds;

    private readonly int prefetchCount;

    private readonly IRetrieverRabbitMqConnectionProvider connectionProvider;

    private readonly SemaphoreSlim lockObj = new SemaphoreSlim(1, 1);

    private readonly List<TaskCompletionSource<object>> consumingFinishedTaskCompletionSources = new List<TaskCompletionSource<object>>();

    private readonly Queue<BasicDeliverEventArgs> consumedMessageObjects = new Queue<BasicDeliverEventArgs>();

    private bool isReConnectionRequired;

    private bool isConsumerReCreationRequired;

    private DateTimeOffset consumerReCreationThreasholdDateTime = DateTimeOffset.MinValue;

    private int numberOfMessagesInProgress = NumberOfInProgressMessagesAvailableForReConnectionOrConsumerReCreation;

    private IModel channel;

    private EventingBasicConsumer eventingBasicConsumer;

    protected BaseRabbitMqRetrieverDataProvider(
        IRetrieverRabbitMqConnectionProvider connectionProvider,
        string retrieverQueueName,
        int prefetchCount,
        int retreiverTimeoutSeconds)
    {
        this.connectionProvider = connectionProvider;
        this.RetrieverQueueName = retrieverQueueName;
        this.prefetchCount = prefetchCount;
        this.retreiverTimeoutSeconds = retreiverTimeoutSeconds;
    }

    public void Dispose()
    {
        connectionProvider.Dispose();
    }

    protected static TRetrieve GetRetrieveMessage<TRetrieve>(
        string retrieveMessageTypeName,
        byte[] retrieveMessageBodyBytes,
        string correlationId,
        Action<TRetrieve> initalizeRetrieveMessage = null)
        where TRetrieve : BaseRetrieve, new()
    {
        var retrieveMessageType = RpcMessagesAssembly.GetType(retrieveMessageTypeName);
        var retrieveMessageBody = RabbitMqRpcMessageEncoderHelper.Decode(retrieveMessageBodyBytes, retrieveMessageType);

        var request = new TRetrieve
        {
            CorrelationId = correlationId,
            RequestBody = retrieveMessageBody
        };

        initalizeRetrieveMessage?.Invoke(request);

        return request;
    }

    protected async Task InitializeChannel(Action<IModel> ensureQueueExist)
    {
        await this.ExecuteWithLockAsync(async () =>
        {
            this.channel = await this.connectionProvider.GetChannel(this.GetConnectionName());

            this.SetChannelSettings();

            // ensure retriever queue exist; retriever queue is the same queu as for sender and should be created with in the same way
            ensureQueueExist(this.channel);

            // create consumer
            this.CreateConsumerWithoutLock();
        });
    }

    protected async Task<TRetrieve> RetrieveBlockingAsync<TRetrieve>(
        Func<BasicDeliverEventArgs, TRetrieve> getRetrieveMessageFn,
        Action<BasicDeliverEventArgs, TRetrieve> initializeRetrieveMessage = null)
        where TRetrieve : BaseRetrieve, new()
    {
        TRetrieve request = null;

        try
        {
            await WaitForFinishingMessagesIfNeeded();
            await ReConnectIfNeeded();
            ReCreateConsumerIfNeeded();

            TRetrieve retrieveMessageConsumed = null;
            TaskCompletionSource<object> consumingFinishedTaskCompletionSource = null;

            ExecuteWithLock(() =>
            {
                if (consumedMessageObjects.Any())
                {
                    // message is already consumed from the queue in advance
                    // dequeue it
                    var consumedMessageObject = consumedMessageObjects.Dequeue();

                    // it is possible situation when message is retrieved by one task and all other tasks are interrupted by timeout; 
                    // as far as message is still received, it means that consumer/connection is alived and there is no need to
                    // recreate consumer; in order to avoid it we shift consumerReCreationThreasholdDateTime 
                    consumerReCreationThreasholdDateTime = DateTimeOffset.UtcNow;

                    // gets retrieve-message
                    retrieveMessageConsumed = getRetrieveMessageFn(consumedMessageObject);
                }
                else
                {
                    // if message wasn't consumed in davance, then start waiting for receiving
                    // task-completions-source is used for singaling the thread to weak up
                    consumingFinishedTaskCompletionSource = new TaskCompletionSource<object>();

                    // the thread might be woken up be cancellation-token
                    var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(this.retreiverTimeoutSeconds));
                    cancellationTokenSource.Token.Register(() => consumingFinishedTaskCompletionSource?.TrySetCanceled());

                    consumingFinishedTaskCompletionSources.Add(consumingFinishedTaskCompletionSource);
                }
            });

            // message is not retreived from the bus and thread should wait
            if (retrieveMessageConsumed == null && consumingFinishedTaskCompletionSource != null)
            {
                // wait for task to be finished (by consumer's event handler or by calncelling)
                var task = consumingFinishedTaskCompletionSource.Task;
                try
                {
                    await task;
                }
                catch (OperationCanceledException)
                {
                    // request re-creation of consumer in order to check that connection is still alive
                    ExecuteWithLock(() =>
                    {
                        if (DateTimeOffset.UtcNow >
                            consumerReCreationThreasholdDateTime.AddSeconds(retreiverTimeoutSeconds * ConsumerReCreationThresholdMultiplier))
                        {
                            isConsumerReCreationRequired = true;
                            consumerReCreationThreasholdDateTime = DateTimeOffset.UtcNow;
                        }
                    });
                }
                finally
                {
                    // set task-completion-source to null and removes it from collection
                    ExecuteWithLock(() =>
                    {
                        if (consumingFinishedTaskCompletionSources.Contains(consumingFinishedTaskCompletionSource))
                        {
                            consumingFinishedTaskCompletionSources.Remove(consumingFinishedTaskCompletionSource);
                        }

                        consumingFinishedTaskCompletionSource = null;
                    });
                }
            }

            // once consumer is cancelled, check whether message was retrieved or not
            ExecuteWithLock(() =>
            {
                // if message is retreived, pass it to caller and increase the number of processing messages
                if (retrieveMessageConsumed != null)
                {
                    request = retrieveMessageConsumed;
                    numberOfMessagesInProgress++;
                }
            });
        }
        catch (Exception exception)
        {
            // if any error happened, then request reconnection during next step
            ExecuteWithLock(() => { isReConnectionRequired = true; });
        }

        return request;
    }

    // message has to be acknowledged as later as possible right before thread is able to process another pre-fetch message
    protected void AcknowledgeMessageImplementation(ulong messageDeliveryTag, Action<IModel> postAcknowledgeAction = null)
    {
        ExecuteWithLock(() =>
        {
            try
            {
                // decrease number of messages which are in progress
                numberOfMessagesInProgress--;

                // acknowledge request message
                channel.BasicAck(messageDeliveryTag, false);

                postAcknowledgeAction?.Invoke(this.channel);
            }
            catch (Exception exception)
            {
            }
        });
    }

    #region Create consumer

    // consumer creation must be wrapped in execution-with-lock by caller
    private void CreateConsumerWithoutLock()
    {
        // initialize consumer for retrieving messages
        eventingBasicConsumer = new EventingBasicConsumer(this.channel);

        // event receiver for consumer
        eventingBasicConsumer.Received += (sender, e) =>
        {
            TaskCompletionSource<object> consumingFinishedTaskCompletionSource = null;
            try
            {
                ExecuteWithLock(() =>
                {
                    // according to QoS settings, it's only possible for consumer to have limited amount of un-acknowledged message at once
                    // gets retrieve-message
                    consumedMessageObjects.Enqueue(e);

                    // gets consumingFinishedTaskCompletionSource from collection, if it exists;
                    // and if it exists, then remove it from collection in order to avoid case when task is awaken but hasn't cleaned up
                    // consumingFinishedTaskCompletionSource from collection yet
                    consumingFinishedTaskCompletionSource = consumingFinishedTaskCompletionSources.FirstOrDefault();
                    if (consumingFinishedTaskCompletionSource != null)
                    {
                        consumingFinishedTaskCompletionSources.Remove(consumingFinishedTaskCompletionSource);
                    }
                });
            }
            catch (Exception exception)
            {
            }
            finally
            {
                // awaking of the waiting-taks can't be wrapped into 'ExecuteWithLock' because rabbit-mq handler's thread might start execution
                // of wait-task right immediately which leads to deadlock
                // try to complete task and continue execution (no exception should be thrown)
                consumingFinishedTaskCompletionSource?.TrySetResult(null);
            }
        };
    }

    #endregion

    #region Wait for finishing in-progress messages

    // wait for finishing in-progress messages, if it's needed
    private async Task WaitForFinishingMessagesIfNeeded()
    {
        // if reconnection is required, try to wait all in-progress messages
        var waitsForFinishingInProgressMessages = false;
        do
        {
            ExecuteWithLock(() => waitsForFinishingInProgressMessages = (isReConnectionRequired || isConsumerReCreationRequired) &&
                                                                             numberOfMessagesInProgress > NumberOfInProgressMessagesAvailableForReConnectionOrConsumerReCreation);
            if (waitsForFinishingInProgressMessages)
            {
                await Task.Delay(TimeSpan.FromSeconds(WaitMessagesToBeFinishedSeconds));
            }
        }
        while (waitsForFinishingInProgressMessages);
    }

    #endregion

    #region Reconnect, if needed

    // re-connect, if it's needed
    private async Task ReConnectIfNeeded()
    {
        await ExecuteWithLockAsync(async () =>
        {
            // handle reconnection
            if (isReConnectionRequired)
            {
                // check if reconnection is required and number of messages which are in progress is zero, then reconnect
                if (numberOfMessagesInProgress == NumberOfInProgressMessagesAvailableForReConnectionOrConsumerReCreation)
                {
                    channel = await connectionProvider.ReConnectAndGetChannel(GetConnectionName());
                    SetChannelSettings();
                    CreateConsumerWithoutLock();
                    isReConnectionRequired = false;

                    // if reconnection happens, there is no need to continue request of consumer-recreation
                    isConsumerReCreationRequired = false;
                }
                else
                {
                    // if reconnection is required but messages are still in progress after delay-period, then throw exception
                    throw new NotSupportedException(NotPossibleToReconnectDueToMessagesInProgress);
                }
            }
        });
    }

    #endregion

    #region Recreate consumer, if needed

    // Re-create consumer if itis needed
    private void ReCreateConsumerIfNeeded()
    {
        ExecuteWithLock(() =>
        {
            // handle reconnection
            if (isConsumerReCreationRequired)
            {
                // check if recreation of consumer is required and number of messages which are in progress is zero, then reconnect
                if (numberOfMessagesInProgress == NumberOfInProgressMessagesAvailableForReConnectionOrConsumerReCreation)
                {
                    // create consumer
                    CreateConsumerWithoutLock();

                    isConsumerReCreationRequired = false;
                }
                else
                {
                    // if reconnection is required but messages are still in progress after delay-period, then throw exception
                    throw new NotSupportedException(NotPossibleToReconnectDueToMessagesInProgress);
                }
            }
        });
    }

    #endregion

    #region Execute with lock methods

    private void ExecuteWithLock(Action exec)
    {
        // wait for lock
        lockObj.Wait();

        try
        {
            exec();
        }
        finally
        {
            // release lock
            lockObj.Release();
        }
    }

    private async Task ExecuteWithLockAsync(Func<Task> exec)
    {
        // asyncronously wait for lock
        await lockObj.WaitAsync();

        try
        {
            await exec();
        }
        finally
        {
            // release lock
            lockObj.Release();
        }
    }

    #endregion

    private void SetChannelSettings()
    {
        // ensures the following: PrefetchSize = 0 means that there is no limitation on message size itself;
        // IsGlobal = false means that prefetch-count is applied per each newly created consumer only;
        // PrefetchCount = 1 means that each consumer recieves only one not-ackwnoledged message and is not able to
        // recieve any new message until the previous one is acknowledged; once it's acknowledged consumer might immediately receieve another one;
        // instead of 1 value of prefetch-count it is used number of workers for prefetching
        channel.BasicQos(PrefetchSize, (ushort)prefetchCount, IsGlobal);
    }

    private string GetConnectionName() => string.Format(RetrieverConnectionNameTemplate, RetrieverQueueName);
}