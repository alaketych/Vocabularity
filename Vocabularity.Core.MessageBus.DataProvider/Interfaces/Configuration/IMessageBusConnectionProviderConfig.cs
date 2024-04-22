namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration;

public interface IMessageBusConnectionProviderConfig
{
    string ConnectionString { get; }

    int EstablishConnectionTimeoutMilliseconds { get; }
}