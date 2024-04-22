using Vocabularity.Core.MessageBus.DataProvider.Interfaces.Configuration;

namespace Vocabularity.Core.MessageBus.DataProvider.Configuration;

public class MessageBusConnectionProviderConfig : IMessageBusConnectionProviderConfig
{
    private const string SectionNameValue = "RpcConnection";

    private MessageBusConnectionProviderConfig()
    {
    }

    public static IMessageBusConnectionProviderConfig Instance { get; } = new MessageBusConnectionProviderConfig();

    public string SectionName => SectionNameValue;

    #region IRpcConnectionProviderConfig interface

    public string ConnectionString { get; set; }

    public int EstablishConnectionTimeoutMilliseconds { get; set; }

    #endregion
}