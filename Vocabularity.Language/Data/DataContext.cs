using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vocabularity.Language.Entities;

namespace Vocabularity.Language.Data
{
	public class DataContext : DbContext
	{
		DbSet<Entities.Language> Languages { get; set; }

		public DataContext(DbContextOptions<DataContext> options) : base(options) 
		{
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
		}
	}
}
