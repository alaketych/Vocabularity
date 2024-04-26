using RabbitMQ.Client;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Interfaces;

public interface ISenderRabbitMqConnectionProvider : IDisposable
{
    Task<IModel> GetChannel(string connnectionName);
}