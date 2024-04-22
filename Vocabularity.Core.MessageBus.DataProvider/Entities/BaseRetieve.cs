namespace Vocabularity.Core.MessageBus.DataProvider.Entities;

public abstract class BaseRetrieve
{
    public string CorrelationId { get; set; }

    public object RequestBody { get; set; }

    /// <summary>
    /// Gets or sets delivery-tag. Delivery-tag is used fo aknowledging messages manually. Messages shouldn't be acknowledged automatically and has to be acknwledged manually.
    /// </summary>
    public ulong DeliveryTag { get; set; }
}