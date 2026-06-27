using Microsoft.EntityFrameworkCore;
using Vocabularity.Core.Data;
using Vocabularity.Core.Entities;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.Dictionary.Interfaces;

namespace Vocabularity.Service.Dictionary.Implementation;

public class DictionaryRepository : Repository<DictionaryEntry>, IDictionaryRepository
{
    public DictionaryRepository(VocabularityDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DictionaryEntry>> GetDictionaryByLanguage(string languageId)
    {
        return await DbSet
            .Where(d => d.LanguageId == languageId)
            .ToListAsync();
    }
}
