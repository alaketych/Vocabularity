using Vocabularity.Core.Interfaces;

namespace Vocabularity.Service.Dictionary.Interfaces;

public interface IDictionaryRepository : IRepository<Entities.Dictionary>
{
    Task<IEnumerable<Entities.Dictionary>> GetDictionaryByLanguage(string languageId);
}
