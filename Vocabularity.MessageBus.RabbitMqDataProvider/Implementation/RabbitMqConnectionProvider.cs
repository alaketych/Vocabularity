using System;
using System.Threading.Tasks;

using RabbitMQ.Client;

using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration;
using Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation;

internal class RabbitMqConnectionProvider : ISenderRabbitMqConnectionProvider, IRetrieverRabbitMqConnectionProvider
{
    private readonly int establishConnectionTimeoutMilliseconds;

    private readonly ConnectionFactory connectionFactory;

    private IConnection connection;

    private IModel channel;

    public RabbitMqConnectionProvider(IMessageBusConnectionProviderConfig connectionProviderConfig)
    {
        establishConnectionTimeoutMilliseconds = connectionProviderConfig.EstablishConnectionTimeoutMilliseconds;
        connectionFactory = new ConnectionFactory()
        {
            Uri = new Uri(connectionProviderConfig.ConnectionString)
        };
    }

    public async Task<IModel> GetChannel(string connnectionName)
    {
        await GetChannelImplementation(connnectionName);
        return channel;
    }

    public async Task<IModel> ReConnectAndGetChannel(string connnectionName)
    {
        CloseImplementation();

        await GetChannelImplementation(connnectionName);
        return channel;
    }

    public void Dispose()
    {
        CloseImplementation();
    }

    #region Connection/channel creation

    private async Task<IModel> GetChannelImplementation(string connnectionName)
    {
        bool ConnectionIsNotOkFn() => connection == null || !connection.IsOpen;
        bool ChannelIsNotOk() => channel == null || !channel.IsOpen;
        while (ConnectionIsNotOkFn() || ChannelIsNotOk())
        {
            try
            {
                if (ConnectionIsNotOkFn())
                {
                    connection = string.IsNullOrWhiteSpace(connnectionName)
                                          ? connectionFactory.CreateConnection()
                                          : connectionFactory.CreateConnection(connnectionName);
                }

                if (ChannelIsNotOk())
                {
                    channel = connection.CreateModel();
                }
            }
            catch (Exception exception)
            {
                await Task.Delay(establishConnectionTimeoutMilliseconds);
            }
        }

        return channel;
    }

    #endregion

    #region Close implementation

    private void CloseImplementation()
    {
        if (channel != null)
        {
            if (channel.IsOpen)
            {
                channel.Close();
            }

            channel.Dispose();
        }

        if (connection != null)
        {
            if (connection.IsOpen)
            {
                connection.Close();
            }

            connection.Dispose();
        }
    }

    #endregion
}