namespace Vocabularity.Core.MessageBus.Interfaces;

public interface IRpcResponse<out TRpcResult> : IErrorRpcResponse
{
    TRpcResult Result { get; }
}
