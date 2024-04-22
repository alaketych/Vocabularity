using System.Collections.Generic;

namespace Vocabularity.Core.MessageBus.Interfaces.ValidationResponse;

public interface IValidationErrorRpcResponse<TError>
{
	List<TError> ValidationErrors { get; set; }
}