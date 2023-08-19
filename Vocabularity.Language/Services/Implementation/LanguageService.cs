using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocabularity.Language.Repositories;
using Vocabularity.Language.Services.Interfaces;

namespace Vocabularity.Language.Services.Implementation
{
    public class LanguageService : ILanguageService
    {
        private readonly LangugageRepository LangugageRepository;

        public LanguageService(LangugageRepository LangugageRepository)
        {
            LangugageRepository = this.LangugageRepository;
        }

        public async Task<IEnumerable<Entities.Language>> GetLanguagesAsync()
        {
            return await this.LangugageRepository.GetAllAsync();
        }

        public async Task<Entities.Language> GetLanguageByIdAsync(int id)
        {
            return await this.LangugageRepository.GetIdAsync(id);
        }

        public async Task<Entities.Language> GetLanguageByCodeAsync(string code)
        {
            return await this.LangugageRepository.GetLanguageByCodeAsync(code);
        }

        public async Task<Entities.Language> UpdateLanguageAsync(Entities.Language language)
        {
            return await this.LangugageRepository.UpdateAsync(language);
        }

        public async Task<bool> DeleteLanguageByIdAsync(int id)
        {
            return await this.LangugageRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteLanguageByCodeAsync(string code)
        {
            return await this.LangugageRepository.DeleteByCodeAsync(code);
        }
    }
}
