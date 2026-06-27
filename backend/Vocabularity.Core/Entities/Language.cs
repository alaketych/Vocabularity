namespace Vocabularity.Core.Entities;

public class Language : Entity
{
    public string Name { get; set; } = string.Empty;

    public string LanguageImage { get; set; } = string.Empty;

    public Language() : base(true)
    {
    }
}
