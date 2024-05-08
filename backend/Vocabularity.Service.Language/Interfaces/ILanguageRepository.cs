using Vocabularity.Core.Interfaces;

namespace Vocabularity.Service.Language.Interfaces;

public interface ILanguageRepository : IRepository<Entities.Language>
{
    Task<IEnumerable<Entities.Language>> GetLanguagesByUser(string userId);
}
