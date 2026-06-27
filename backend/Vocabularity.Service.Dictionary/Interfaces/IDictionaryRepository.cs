using Vocabularity.Core.Interfaces;
using Vocabularity.Core.Entities;

namespace Vocabularity.Service.Dictionary.Interfaces;

public interface IDictionaryRepository : IRepository<DictionaryEntry>
{
    Task<IEnumerable<DictionaryEntry>> GetDictionaryByLanguage(string languageId);
}
