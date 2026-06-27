using Vocabularity.Core.Interfaces;
using LanguageEntity = Vocabularity.Core.Entities.Language;

namespace Vocabularity.Service.Language.Interfaces;

public interface ILanguageRepository : IRepository<LanguageEntity>
{
    Task<IEnumerable<LanguageEntity>> GetLanguagesByUser(string userId);
}
