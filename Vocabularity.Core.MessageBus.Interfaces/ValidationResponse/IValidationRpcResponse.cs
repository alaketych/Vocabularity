namespace Vocabularity.Core.MessageBus.Interfaces.ValidationResponse;

public interface IValidationRpcResponse<TError, out TRpcResult> : IValidationErrorRpcResponse<TError>, IRpcResponse<TRpcResult>
    where TError : IValidationErrorDetailsRpcResponse
{
}
