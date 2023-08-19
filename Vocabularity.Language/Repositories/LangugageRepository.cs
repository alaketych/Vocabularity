using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vocabularity.Language.Data;

namespace Vocabularity.Language.Repositories
{
    public class LangugageRepository : Repository<Entities.Language, DataContext>
    {
        private readonly DataContext _context;

        public LangugageRepository(DataContext context) 
            : base(context)
        {
            _context = context;
        }
        public async Task<Entities.Language> GetLanguageByCodeAsync(string code)
        {
            return await _context.Set<Entities.Language>().FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<bool> DeleteByCodeAsync(string code)
        {
            bool status;

            try
            {
                Entities.Language entity = await _context.Set<Entities.Language>().FindAsync(code);

                if (entity != null)
                {
                    _context.Set<Entities.Language>().Remove(entity);
                    await _context.SaveChangesAsync();
                }

                status = true;
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }
    }
}
