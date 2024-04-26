using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client.Framing;

using Vocabularity.Core.MessageBus.DataProvider.Entities;
using Vocabularity.Core.MessageBus.DataProvider.Entities.Interfaces;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration.Rpc;
using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Rpc;
using Vocabularity.Core.MessageBus.Interfaces;
using Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Helpers;
using Vocabularity.MessageBus.RabbitMqDataProvider.Implementation;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Rpc;

public class RabbitMqRpcRetrieverDataProvider : BaseRabbitMqRetrieverDataProvider, IRpcRetrieverDataProvider
{
    public RabbitMqRpcRetrieverDataProvider(
        IRetrieverRabbitMqConnectionProvider connectionProvider,
        IRpcRetrieverDataProviderConfig retrieverDataProviderConfig)
        : base(
            connectionProvider,
            retrieverDataProviderConfig.RetrieverQueueName,
            retrieverDataProviderConfig.PrefetchMessagesCount,
            retrieverDataProviderConfig.RetrieverTimeoutSeconds)
    {
    }

    public async Task InitializeChannel()
    {
        await InitializeChannel(channel =>
        {
            // ensure retriever queue exist; retriever queue is the same queue as for sender and should be created with in the same way
            RabbitMqRpcSenderDataProvider.EnsureSenderQueueExists(channel, RetrieverQueueName);
        });
    }

    public async Task<RpcRetrieveRequest> RetrieveBlockingAsync()
    {
        var rpcRetrieveRequest = await RetrieveBlockingAsync(
                                     basicDeliverEventArgs => GetRetrieveRequest(
                                         basicDeliverEventArgs.Body,
                                         basicDeliverEventArgs.DeliveryTag,
                                         basicDeliverEventArgs.BasicProperties.CorrelationId,
                                         basicDeliverEventArgs.BasicProperties.ReplyTo,
                                         GetResponseTypeName(basicDeliverEventArgs.BasicProperties.Headers),
                                         basicDeliverEventArgs.BasicProperties.Type));
        return rpcRetrieveRequest;
    }

    public async Task ReplyAsync(object response, IRpcRetrieveRequest retrieveRequest)
    {
        await Task.Run(
            () =>
            {
                AcknowledgeMessageImplementation(
                    retrieveRequest.DeliveryTag,
                channel =>
                    {
                        // sends response
                        byte[] responseBytes = RabbitMqRpcMessageEncoderHelper.Encode(response);
                        channel.BasicPublish(
                                RabbitMqSenderDataProviderConstants.DirectExchange,
                                retrieveRequest.ReplyTo,
                                false,
                                new BasicProperties
                                {
                                    CorrelationId = retrieveRequest.CorrelationId
                                },
                                responseBytes);
                    });
            });
    }

    private static string GetResponseTypeName(IDictionary<string, object> basicPropertiesHeaders)
    {
        var responseTypeName = Encoding.UTF8.GetString((byte[])basicPropertiesHeaders[RabbitMqRpcSenderDataProvider.ResponseTypeHeaderName]);
        return responseTypeName;
    }

    private static RpcRetrieveRequest GetRetrieveRequest(
        byte[] requestBodyBytes,
        ulong deliveryTag,
        string correlationId,
        string replyTo,
        string responseMessageTypeName,
        string requestMessageTypeName)
    {
        var retrieveRequest = GetRetrieveMessage<RpcRetrieveRequest>(
            requestMessageTypeName,
            requestBodyBytes,
            correlationId,
            retrieveMessage =>
            {
                // set delivery-tag property
                retrieveMessage.DeliveryTag = deliveryTag;

                // set reply-to property
                retrieveMessage.ReplyTo = replyTo;

                // set error-response property
                var responseMessageType = RpcMessagesAssembly.GetType(responseMessageTypeName);
                var errorResponse = (IErrorRpcResponse)Activator.CreateInstance(responseMessageType);
                retrieveMessage.EmptyErrorResponse = errorResponse;
            });
        return retrieveRequest;
    }
}