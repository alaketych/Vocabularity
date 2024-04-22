using Vocabularity.Core.MessageBus.DataProvider.Entities.Interfaces;
using Vocabularity.Core.MessageBus.Interfaces;
using Vocabularity.Core.MessageBus.DataProvider.Entities;

namespace Vocabularity.Core.MessageBus.DataProvider.Entities;

public class RpcRetrieveRequest : BaseRetrieve, IRpcRetrieveRequest
{
    public string ReplyTo { get; set; }

    public IErrorRpcResponse EmptyErrorResponse { get; set; }
}