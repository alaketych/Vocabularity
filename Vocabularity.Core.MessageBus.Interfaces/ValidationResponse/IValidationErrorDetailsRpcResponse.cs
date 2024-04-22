namespace Vocabularity.Core.MessageBus.Interfaces.ValidationResponse;

public interface IValidationErrorDetailsRpcResponse
{
    int Code { get; set; }

    string Message { get; set; }
}
