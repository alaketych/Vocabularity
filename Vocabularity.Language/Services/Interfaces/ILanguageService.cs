using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vocabularity.Language.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<IEnumerable<Entities.Language>> GetLanguagesAsync();

        Task<Entities.Language> GetLanguageByIdAsync(int id);

        Task<Entities.Language> GetLanguageByCodeAsync(string code);

        Task<Entities.Language> UpdateLanguageAsync(Entities.Language language);
        
        Task<bool> DeleteLanguageByIdAsync(int id);

        Task<bool> DeleteLanguageByCodeAsync(string code);
    }
}
