using Vocabularity.Core.MessageBus.DataProvider.Entities;
using Vocabularity.Core.MessageBus.DataProvider.Entities.Interfaces;

namespace Vocabularity.Core.MessageBus.DataProvider.Interfaces.Rpc;

public interface IRpcRetrieverDataProvider : IDisposable
{
    Task InitializeChannel();

    Task<RpcRetrieveRequest> RetrieveBlockingAsync();

    Task ReplyAsync(object response, IRpcRetrieveRequest retrieveRequest);
}