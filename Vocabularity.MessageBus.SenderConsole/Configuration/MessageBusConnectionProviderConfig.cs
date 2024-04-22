using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration;

namespace Vocabularity.MessageBus.SenderConsole.Configuration;

public class MessageBusConnectionProviderConfig : IMessageBusConnectionProviderConfig
{
    private MessageBusConnectionProviderConfig()
    {
    }

    public string ConnectionString => "amqp://admin:admin@localhost:5672";

    public int EstablishConnectionTimeoutMilliseconds => 10000;

    internal static IMessageBusConnectionProviderConfig Instance { get; } = new MessageBusConnectionProviderConfig();
}