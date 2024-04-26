using RabbitMQ.Client;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

public interface IRetrieverRabbitMqConnectionProvider : IDisposable
{
    Task<IModel> GetChannel(string connnectionName);

    Task<IModel> ReConnectAndGetChannel(string connnectionName);
}