namespace Vocabularity.Core.MessageBus.DataProvider.Entities.Interfaces;

public interface IRpcRetrieveRequest
{
    ulong DeliveryTag { get; }

    string ReplyTo { get; }

    string CorrelationId { get; }
}