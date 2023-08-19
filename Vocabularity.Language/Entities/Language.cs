using Vocabularity.Core.Domain.Interfaces;

namespace Vocabularity.Language.Entities
{
	public class Language : IEntity
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Code { get; set; }
	}
}
