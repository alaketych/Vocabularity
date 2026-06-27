using Microsoft.EntityFrameworkCore;
using Vocabularity.Core.Data;
using Vocabularity.Core.Implementation;
using Vocabularity.Service.Language.Interfaces;
using LanguageEntity = Vocabularity.Core.Entities.Language;

namespace Vocabularity.Service.Language.Implementation;

public class LanguageRepository : Repository<LanguageEntity>, ILanguageRepository
{
    public LanguageRepository(VocabularityDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LanguageEntity>> GetLanguagesByUser(string userId)
    {
        return await DbSet.ToListAsync();
    }
}
